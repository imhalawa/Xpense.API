using System.Text.Json;
using Xpense.Domain.Entities;
using Xpense.Domain.Events;

namespace Xpense.Notifications.Rules;

/// <summary>
/// Turns a stored event row back into a typed event and asks every rule for that type.
/// <para>
/// The non-generic face exists because the pump reads rows, and a row carries its type as a string.
/// Something has to bridge that to a generic <see cref="Event{TBody}"/>, and doing it here keeps
/// reflection out of both the pump and the rules.
/// </para>
/// </summary>
public interface IEventDispatcher
{
    /// <summary>The body type name this handles, matched against <see cref="EventRecord.Type"/>.</summary>
    string EventType { get; }

    Task<IReadOnlyList<NotificationDraft>> Dispatch(EventRecord record, CancellationToken ct);
}

/// <summary>
/// The dispatcher for one body type. Registered once per body type that has at least one rule, so an
/// event nobody wrote a rule for has no dispatcher and is processed to no effect.
/// </summary>
public sealed class EventDispatcher<TBody>(IEnumerable<INotificationRule<TBody>> rules) : IEventDispatcher
    where TBody : EventBody
{
    public string EventType => typeof(TBody).Name;

    public async Task<IReadOnlyList<NotificationDraft>> Dispatch(EventRecord record, CancellationToken ct)
    {
        var body = JsonSerializer.Deserialize<TBody>(record.Body, EventJson.Options)
                   ?? throw new InvalidOperationException(
                       $"Event {record.EventId} of type {record.Type} deserialized to null.");

        var @event = new Event<TBody>(
            new EventAttributes(record.EventId, record.Type, record.OccurredAt, record.Source, record.Version),
            body);

        var drafts = new List<NotificationDraft>();

        // Sequentially, sharing one DbContext across rules through DI. Running them concurrently would
        // mean concurrent use of a scoped DbContext, which EF forbids -- and six quick queries do not
        // need the parallelism.
        foreach (var rule in rules)
            drafts.AddRange(await rule.Evaluate(@event, ct));

        return drafts;
    }
}
