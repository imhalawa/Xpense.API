using System.Text.Json;
using Xpense.Domain.Entities;
using Xpense.Domain.Events;

namespace Xpense.Persistence;

/// <summary>
/// Emits events by inserting them into the <c>Events</c> table through the caller's DbContext.
/// <para>
/// Deliberately does not call SaveChanges. The insert joins whatever transaction the caller is
/// already in, so the event and the thing it describes commit together or not at all -- there is no
/// window where Xpense has announced something it failed to record, and none where it recorded
/// something it will never announce. That is the whole reason the queue is a table.
/// </para>
/// </summary>
public sealed class EventBus(XpenseDbContext db) : IEventBus
{
    /// <summary>
    /// Bodies are serialized with the default options, which for a fixed record type emits properties
    /// in declaration order. Nothing depends on that ordering today, but it is also what keeps a
    /// hash over serialized output stable, so it is worth not changing casually.
    /// </summary>
    internal static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public Task Emit<TBody>(Event<TBody> @event, CancellationToken ct = default)
        where TBody : EventBody
    {
        var body = JsonSerializer.Serialize(@event.Body, Json);

        db.Events.Add(EventRecord.Of(@event, body));

        // Nothing awaits: Add is synchronous, and the write happens when the caller saves. The task
        // return type is here so this can grow an await later without changing every call site.
        return Task.CompletedTask;
    }
}
