using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xpense.Domain.Entities;
using Xpense.Notifications.Rules;
using Xpense.Persistence;

namespace Xpense.Notifications;

public sealed class EventProcessor(
    XpenseDbContext dbContext,
    IEnumerable<IEventDispatcher> dispatchers,
    ILogger<EventProcessor> logger)
{
    public const int BatchSize = 20;

    private readonly Dictionary<string, IEventDispatcher> dispatchers =
        dispatchers.ToDictionary(dispatcher => dispatcher.EventType);

    public async Task<int> ProcessBatch(CancellationToken cancellationToken = default)
    {
        // One transaction around claim-and-process: the row locks below are held until it ends, and a
        // crash mid-batch rolls back so the events stay outstanding rather than half-handled.
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var claimed = await Claim(cancellationToken);

        if (claimed.Count == 0)
            return 0;

        foreach (var record in claimed)
            await Process(record, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return claimed.Count;
    }

    private Task<List<EventRecord>> Claim(CancellationToken cancellationToken) =>
        dbContext.Events
            .FromSql($"""
                      SELECT * FROM "Xpense"."Events"
                      WHERE "ProcessedAt" IS NULL AND "IsDeleted" = false
                      ORDER BY "CreatedAt"
                      LIMIT {BatchSize}
                      FOR UPDATE SKIP LOCKED
                      """)
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);

    private async Task Process(EventRecord record, CancellationToken cancellationToken)
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
            var drafts = await dispatcher.Dispatch(record, cancellationToken);
            var stored = await Store(record, drafts, cancellationToken);

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
        CancellationToken cancellationToken)
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
        var known = await dbContext.Notifications
            .Where(notification => notification.EventId == record.EventId
                                   && hashes.Contains(notification.PayloadHash))
            .Select(notification => notification.PayloadHash)
            .ToListAsync(cancellationToken);

        var fresh = candidates.Where(candidate => !known.Contains(candidate.hash)).ToArray();

        foreach (var (draft, payload, hash) in fresh)
        {
            dbContext.Notifications.Add(new Notification
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

    private static string Hash(string payload) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));

    private static string Describe(Exception exception)
    {
        var root = exception.GetBaseException();
        var text = ReferenceEquals(root, exception)
            ? $"{exception.GetType().Name}: {exception.Message}"
            : $"{exception.GetType().Name}: {exception.Message} -> {root.GetType().Name}: {root.Message}";

        return text.Length <= 2000 ? text : text[..2000];
    }

    private void Discard()
    {
        foreach (var entry in dbContext.ChangeTracker.Entries().ToArray())
        {
            if (entry.Entity is not EventRecord)
                entry.State = EntityState.Detached;
        }
    }
}
