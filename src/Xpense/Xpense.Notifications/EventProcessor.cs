using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xpense.Domain.Entities;
using Xpense.Notifications.Rules;
using Xpense.Persistence;

namespace Xpense.Notifications;

/// <summary>
/// Claims a batch of outstanding events, asks the rules what is worth telling anyone, and stores the
/// answers. One batch per call, so what happens is separate from when it happens -- <see cref="EventPump"/>
/// decides the latter, and a test can drive this directly rather than starting a background loop.
/// <para>
/// There is no broker: the Events table is the queue. See
/// docs/adr/0008-the-events-table-is-the-queue.md.
/// </para>
/// </summary>
public sealed class EventProcessor(
    XpenseDbContext db,
    IEnumerable<IEventDispatcher> dispatchers,
    ILogger<EventProcessor> logger)
{
    public const int BatchSize = 20;

    private readonly Dictionary<string, IEventDispatcher> dispatchers =
        dispatchers.ToDictionary(dispatcher => dispatcher.EventType);

    /// <summary>
    /// Processes up to <see cref="BatchSize"/> events and returns how many were claimed. A full batch
    /// means there is probably more waiting.
    /// </summary>
    public async Task<int> ProcessBatch(CancellationToken ct = default)
    {
        // One transaction around claim-and-process: the row locks below are held until it ends, and a
        // crash mid-batch rolls back so the events stay outstanding rather than half-handled.
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var claimed = await Claim(ct);

        if (claimed.Count == 0)
            return 0;

        foreach (var record in claimed)
            await Process(record, ct);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return claimed.Count;
    }

    /// <summary>
    /// Takes the oldest outstanding events, locking them so nothing else picks them up.
    /// <para>
    /// SKIP LOCKED rather than waiting: a second worker takes the next unlocked rows instead of
    /// blocking on the first one's batch. Only one worker runs today, so this is insurance -- but it is
    /// the difference between scaling out being configuration and being a rewrite.
    /// </para>
    /// <para>
    /// Query filters are ignored so EF does not wrap this in a subquery, which would move the row lock
    /// away from the rows being selected. The IsDeleted condition is therefore written by hand.
    /// </para>
    /// </summary>
    private Task<List<EventRecord>> Claim(CancellationToken ct) =>
        db.Events
            .FromSql($"""
                      SELECT * FROM "Xpense"."Events"
                      WHERE "ProcessedAt" IS NULL AND "IsDeleted" = false
                      ORDER BY "CreatedAt"
                      LIMIT {BatchSize}
                      FOR UPDATE SKIP LOCKED
                      """)
            .IgnoreQueryFilters()
            .ToListAsync(ct);

    private async Task Process(EventRecord record, CancellationToken ct)
    {
        if (!dispatchers.TryGetValue(record.Type, out var dispatcher))
        {
            // Nobody wrote a rule for this kind. A normal state, not an error: producers state facts
            // without caring who listens. Marked done so it is not claimed forever.
            record.Succeeded();
            return;
        }

        try
        {
            var drafts = await dispatcher.Dispatch(record, ct);
            var stored = await Store(record, drafts, ct);

            record.Succeeded();

            if (stored > 0)
                logger.LogInformation(
                    "Event {EventId} ({Type}) produced {Count} notification(s)",
                    record.EventId, record.Type, stored);
        }
        catch (Exception exception)
        {
            // The batch is not abandoned for one bad event: this row records its own failure and is
            // retried next time, up to EventRecord.MaxAttempts.
            logger.LogError(
                exception,
                "Event {EventId} ({Type}) failed on attempt {Attempt}",
                record.EventId, record.Type, record.Attempts + 1);

            record.Failed(Describe(exception));

            // A rule may have left half-built entities tracked before throwing. Detaching them stops
            // the SaveChanges at the end of the batch from writing a partial result.
            Discard();
        }
    }

    private async Task<int> Store(
        EventRecord record,
        IReadOnlyList<NotificationDraft> drafts,
        CancellationToken ct)
    {
        if (drafts.Count == 0)
            return 0;

        var candidates = drafts
            .Select(draft =>
            {
                var payload = JsonSerializer.Serialize(draft.Payload, EventJson.Options);
                return (draft, payload, hash: Hash(payload));
            })
            // Two rules producing identical facts for one event is one notification, not two. Deduping
            // here as well as in the database keeps SaveChanges from failing on our own batch.
            .GroupBy(candidate => candidate.hash)
            .Select(group => group.First())
            .ToArray();

        var hashes = candidates.Select(candidate => candidate.hash).ToArray();

        // The unique index is the real guarantee. This only avoids the exception in the ordinary case
        // where an event is redelivered after already being handled.
        var known = await db.Notifications
            .Where(notification => notification.EventId == record.EventId
                                   && hashes.Contains(notification.PayloadHash))
            .Select(notification => notification.PayloadHash)
            .ToListAsync(ct);

        var fresh = candidates.Where(candidate => !known.Contains(candidate.hash)).ToArray();

        foreach (var (draft, payload, hash) in fresh)
        {
            db.Notifications.Add(new Notification
            {
                EventId = record.EventId,
                Kind = draft.Kind,
                Title = draft.Title,
                Message = draft.Message,
                Payload = payload,
                PayloadHash = hash,
                CreatedAt = DateTime.UtcNow
            });
        }

        return fresh.Length;
    }

    /// <summary>SHA-256 of the serialized facts, lower-case hex. 64 characters, always.</summary>
    private static string Hash(string payload) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));

    /// <summary>
    /// The message plus the innermost cause, which is usually where the real reason is. Truncated
    /// rather than letting the write that was recording a failure fail on column length.
    /// </summary>
    private static string Describe(Exception exception)
    {
        var root = exception.GetBaseException();
        var text = ReferenceEquals(root, exception)
            ? $"{exception.GetType().Name}: {exception.Message}"
            : $"{exception.GetType().Name}: {exception.Message} -> {root.GetType().Name}: {root.Message}";

        return text.Length <= 2000 ? text : text[..2000];
    }

    /// <summary>Detaches everything except the event rows, whose failure state must survive.</summary>
    private void Discard()
    {
        foreach (var entry in db.ChangeTracker.Entries().ToArray())
        {
            if (entry.Entity is not EventRecord)
                entry.State = EntityState.Detached;
        }
    }
}
