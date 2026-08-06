namespace Xpense.Domain.Events;

/// <summary>
/// The facts particular to one kind of event. Everything publishable derives from this.
/// <para>
/// Bodies hold primitives, enums and ids only -- never an entity, and never a value object that
/// wraps one. A body is a wire format: it is serialized, stored, and read back possibly much later,
/// so pinning it to the schema means a renamed property stops an unprocessed event from
/// deserializing at all. <c>EventContractTests</c> enforces this rather than trusting the comment.
/// </para>
/// </summary>
public abstract record EventBody;
