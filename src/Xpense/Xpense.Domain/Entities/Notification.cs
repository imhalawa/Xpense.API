using Xpense.Domain.Enums;

namespace Xpense.Domain.Entities;

/// <summary>
/// Something a user should be told about, decided from an event.
/// <para>
/// Not the same thing as an event, and the distinction is load-bearing: an event is a fact stated by
/// whichever part of Xpense the thing happened in, published whether or not anyone cares, while this
/// is a judgement made afterwards by a rule. Most events produce none of these.
/// </para>
/// <para>
/// Carries the facts and text rendering them. The facts are what it is -- what a client links from,
/// groups by, or acts on. The text exists so anything that only needs to display it can, without
/// knowing the kind.
/// </para>
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// Which user this is for. Always null today: there is no User in the model and no
    /// authentication, so nothing scopes by it. Present because a notification belongs to somebody
    /// the moment there is more than one somebody, and no foreign key exists to point at yet.
    /// </summary>
    public int? OwnerId { get; set; }

    /// <summary>The event this was decided from. Not unique on its own -- one event can warrant several.</summary>
    public Guid EventId { get; set; }

    public NotificationKind Kind { get; set; }

    /// <summary>One line, enough on its own.</summary>
    public required string Title { get; set; }

    /// <summary>The detail behind the title, still a sentence a person can read.</summary>
    public required string Message { get; set; }

    /// <summary>The facts, serialized. What a client acts on when it wants more than the words.</summary>
    public required string Payload { get; set; }

    /// <summary>
    /// A hash of <see cref="Payload"/>, which together with <see cref="EventId"/> is what makes this
    /// notification unique.
    /// <para>
    /// One event can warrant several notifications -- an expense crossing both a weekly and a monthly
    /// budget is two, with different payloads -- so the event alone cannot be the key. Hashing the
    /// facts means a redelivered event reproduces the same hashes and inserts nothing, without anyone
    /// having to invent a naming convention for what each notification is "about".
    /// </para>
    /// <para>
    /// This only holds while payloads contain nothing time-varying. Put a "generated at" inside one
    /// and its hash changes on every delivery, so deduplication silently stops working.
    /// </para>
    /// </summary>
    public required string PayloadHash { get; set; }

    /// <summary>When the recipient saw this. Null means unread -- a timestamp rather than a flag, so
    /// "when did I see it" is answerable and it matches how UpdatedAt is treated.</summary>
    public DateTime? ReadAt { get; set; }

    public void MarkAsRead()
    {
        // Idempotent on purpose: marking twice must not move the timestamp, so the first sighting is
        // the one recorded and read-all over a mixed list leaves already-read rows alone.
        if (ReadAt is not null)
            return;

        ReadAt = ToStorablePrecision(DateTime.UtcNow);
        Touch();
    }

    /// <summary>
    /// Drops the sub-microsecond part of a timestamp, because Postgres cannot keep it.
    /// <para>
    /// A .NET <see cref="DateTime"/> counts 100-nanosecond ticks; <c>timestamptz</c> stores
    /// microseconds. Without this, the response to the request that marked a notification read carries
    /// a more precise value than the one every later read returns, so a client comparing them sees the
    /// timestamp change on a call that changed nothing.
    /// </para>
    /// </summary>
    private static DateTime ToStorablePrecision(DateTime value)
    {
        const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
        return new DateTime(value.Ticks - value.Ticks % ticksPerMicrosecond, value.Kind);
    }
}
