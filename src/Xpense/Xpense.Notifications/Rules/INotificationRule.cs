using Xpense.Domain.Events;

namespace Xpense.Notifications.Rules;

/// <summary>
/// Decides, from one event, whether notifications of one kind are warranted.
/// <para>
/// One rule per kind, in its own file, holding its detection, its payload and its wording. Rules are
/// found by scanning this assembly, so adding a kind means adding a file and changing nothing else --
/// the same arrangement <c>IEndpoint</c> uses for slices.
/// </para>
/// <para>
/// A rule knows nothing about any other rule, and <c>NotificationRuleIsolationTests</c> enforces
/// that. Rules therefore duplicate each other's queries, which is the intended state here for the
/// same reason AGENTS.md gives for slices: independence is worth more than sharing at this size.
/// </para>
/// <para>
/// Dependencies come through the constructor, so a rule takes an <c>XpenseDbContext</c> if it needs
/// one. Returning an empty list is the normal outcome -- most events warrant nothing.
/// </para>
/// </summary>
public interface INotificationRule<TBody> where TBody : EventBody
{
    Task<IReadOnlyList<NotificationDraft>> Evaluate(Event<TBody> @event, CancellationToken ct);
}
