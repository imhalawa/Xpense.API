namespace Xpense.Domain.Events;

/// <summary>
/// How anything states that something happened.
/// <para>
/// <see cref="Emit{TBody}"/> does not send: it records the event alongside whatever else the caller
/// is writing, so the event becomes durable exactly when the caller's transaction commits and not
/// before. A caller that never saves has emitted nothing, and a caller whose save fails has emitted
/// nothing -- there is no window in which Xpense has announced something it did not record.
/// </para>
/// <para>
/// Delivery is somebody else's problem, deliberately. Nothing a producer does depends on a consumer
/// being reachable, or existing.
/// </para>
/// </summary>
public interface IEventBus
{
    Task Emit<TBody>(Event<TBody> @event, CancellationToken ct = default)
        where TBody : EventBody;
}
