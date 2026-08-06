namespace Xpense.Domain.Events;

/// <summary>
/// An event: metadata plus the facts, kept apart so that everything generic about publishing,
/// storing and routing can be written once against the attributes without knowing any body.
/// <para>
/// Generic rather than holding an <see cref="EventBody"/> directly, because the closed type is what
/// distinguishes one kind of event from another for dispatch. A single non-generic envelope would
/// mean one handler for everything, switching on the body's runtime type by hand.
/// </para>
/// </summary>
public sealed record Event<TBody>(EventAttributes Attributes, TBody Body)
    where TBody : EventBody;

/// <summary>
/// Builds events, so no caller assembles <see cref="EventAttributes"/> by hand.
/// <para>
/// Non-generic alongside the generic <see cref="Event{TBody}"/>, the way <c>Task</c> sits beside
/// <c>Task&lt;T&gt;</c>.
/// </para>
/// </summary>
public static class Event
{
    /// <summary>What raised an event, unless a caller says otherwise.</summary>
    public const string DefaultSource = "Xpense.API";

    /// <summary>The current shape of every body. Bump when one of them changes incompatibly.</summary>
    public const int CurrentVersion = 1;

    /// <summary>
    /// Wraps a body, stamping it with a fresh identity and the current time.
    /// <para>
    /// <paramref name="occurredAt"/> exists because "when it happened" is not always "now": a
    /// backdated transaction happened when the money moved. It defaults to now because for most
    /// events the two are the same.
    /// </para>
    /// </summary>
    public static Event<TBody> Of<TBody>(
        TBody body,
        DateTime? occurredAt = null,
        string source = DefaultSource)
        where TBody : EventBody =>
        new(
            new EventAttributes(
                // Version 7: time-ordered, so the index on EventId stays dense as rows arrive.
                Guid.CreateVersion7(),
                typeof(TBody).Name,
                occurredAt ?? DateTime.UtcNow,
                source,
                CurrentVersion),
            body);
}
