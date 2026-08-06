namespace Xpense.Domain.Events;

/// <summary>
/// Everything true of an event regardless of what happened: which one it is, what kind, when, what
/// raised it, and which version of that kind's shape it was written against.
/// </summary>
/// <param name="EventId">
/// Identity, and the thing a consumer deduplicates on. A version 7 GUID, so it sorts by creation
/// time and an index on it stays dense -- a random GUID scatters inserts across the whole index.
/// </param>
/// <param name="Type">
/// The body's type name, which is how a stored event is routed back to the right body to
/// deserialize into. Renaming a body type is therefore a breaking change to anything unprocessed.
/// </param>
/// <param name="OccurredAt">When the thing happened, in UTC. Not when it was delivered or handled.</param>
/// <param name="Source">What raised it, so a consumer can tell the API apart from a script or a job.</param>
/// <param name="Version">
/// The shape of this kind of body. Nothing reads it yet; it exists so that the first time a body
/// gains or loses a field, a consumer has something to branch on other than guesswork.
/// </param>
public sealed record EventAttributes(
    Guid EventId,
    string Type,
    DateTime OccurredAt,
    string Source,
    int Version);
