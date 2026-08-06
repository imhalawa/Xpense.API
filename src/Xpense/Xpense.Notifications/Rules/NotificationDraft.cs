using Xpense.Domain.Enums;

namespace Xpense.Notifications.Rules;

/// <summary>
/// A notification a rule has decided is warranted, before it is stored. The pump serializes the
/// payload, hashes it, stamps the event id and saves -- so a rule never touches persistence concerns.
/// </summary>
/// <param name="Payload">
/// The facts, as any serializable object -- usually a record declared beside the rule that produced
/// it. Must contain nothing time-varying: the hash of this is half the deduplication key, so a
/// timestamp inside it would differ on every delivery and duplicates would stop being caught.
/// </param>
public sealed record NotificationDraft(
    NotificationKind Kind,
    string Title,
    string Message,
    object Payload);
