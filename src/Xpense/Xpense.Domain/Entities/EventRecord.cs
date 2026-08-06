using Xpense.Domain.Events;

namespace Xpense.Domain.Entities;

/// <summary>
/// A published event, as a row. This table is the queue: there is no broker, because Xpense already
/// runs one Postgres and a table gives durability, ordering and retries without a fourth container
/// to operate. See docs/adr/0008-the-events-table-is-the-queue.md.
/// <para>
/// The attributes are flattened into columns rather than kept as JSON so they can be queried and
/// indexed; only the body stays opaque, because only its owner knows its shape.
/// </para>
/// </summary>
public class EventRecord : BaseEntity
{
    /// <summary>The event's own identity, from its attributes. Unique, and what consumers dedupe on.</summary>
    public Guid EventId { get; set; }

    /// <summary>The body's type name, which is how <see cref="Body"/> is routed back to a type.</summary>
    public required string Type { get; set; }

    /// <summary>When the thing happened. Distinct from <see cref="BaseEntity.CreatedAt"/>, the row's time.</summary>
    public DateTime OccurredAt { get; set; }

    public required string Source { get; set; }

    public int Version { get; set; }

    /// <summary>The serialized <see cref="EventBody"/>.</summary>
    public required string Body { get; set; }

    /// <summary>
    /// When this event was dealt with. Null means outstanding, and outstanding is what the worker
    /// claims. Processed rows are kept rather than deleted, so what was published stays auditable and
    /// can be replayed by hand.
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// How many times processing has been tried. A row that keeps failing is abandoned rather than
    /// retried forever -- see <see cref="MaxAttempts"/>.
    /// </summary>
    public int Attempts { get; set; }

    /// <summary>Why the last attempt failed, kept so a stuck event can be diagnosed from the table alone.</summary>
    public string? LastError { get; set; }

    /// <summary>
    /// After this many failures the event is marked processed with its error left in place, so one
    /// poisonous row cannot block the queue forever. There is no dead-letter table: the row itself is
    /// the record, distinguishable by having both a ProcessedAt and a LastError.
    /// </summary>
    public const int MaxAttempts = 5;

    public static EventRecord Of<TBody>(Event<TBody> @event, string body) where TBody : EventBody =>
        new()
        {
            EventId = @event.Attributes.EventId,
            Type = @event.Attributes.Type,
            OccurredAt = @event.Attributes.OccurredAt,
            Source = @event.Attributes.Source,
            Version = @event.Attributes.Version,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

    public void Succeeded()
    {
        ProcessedAt = DateTime.UtcNow;
        LastError = null;
        Touch();
    }

    /// <summary>
    /// Records a failure. The row stays outstanding so it is tried again, unless it has now failed
    /// often enough to be given up on.
    /// </summary>
    public void Failed(string error)
    {
        Attempts++;
        LastError = error;

        if (Attempts >= MaxAttempts)
            ProcessedAt = DateTime.UtcNow;

        Touch();
    }
}
