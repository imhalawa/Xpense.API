using Microsoft.EntityFrameworkCore;
using Xpense.Domain.Entities;
using Xpense.Domain.Enums;
using Xpense.Domain.Events;
using Xpense.Domain.ValueObjects;
using Xpense.Persistence;

namespace Xpense.Notifications.Rules;

/// <summary>
/// Says something the first time spending passes a budget's limit in a period.
/// <para>
/// Fires on the crossing, not on the state. The event carries the amount, so the totals both before
/// and after this expense are known, and the rule fires only when the limit sits between them --
/// which means it says this once per period however many further expenses land. Spending more while
/// already over is a different thing that happened, and a different rule's business.
/// </para>
/// <para>
/// Knows nothing about any other rule and shares no query with one. That is deliberate: see
/// <see cref="INotificationRule{TBody}"/>.
/// </para>
/// </summary>
public sealed class BudgetExceededRule(XpenseDbContext db) : INotificationRule<TransactionRecorded>
{
    public async Task<IReadOnlyList<NotificationDraft>> Evaluate(
        Event<TransactionRecorded> @event,
        CancellationToken ct)
    {
        var body = @event.Body;

        // Only expenses count against a budget. A transfer is money you still own and carries no
        // category; income is not spending. Both are checked because the kind and the category are
        // independent facts on the wire.
        if (body.Kind != TransactionKind.Expense || body.CategoryId is null)
            return [];

        var budgets = await db.Budgets
            .AsNoTracking()
            .Include(budget => budget.Category)
            .Where(budget => budget.CategoryId == body.CategoryId && budget.Currency == body.Currency)
            .ToListAsync(ct);

        if (budgets.Count == 0)
            return [];

        var drafts = new List<NotificationDraft>();

        foreach (var budget in budgets)
        {
            // Which period this expense falls in, for this budget. Null means the budget was not
            // measuring when the money moved -- before it started, after it ended, or a one-off window
            // this expense sits outside.
            var period = budget.PeriodOn(body.OccurredAt);

            if (period is null)
                continue;

            var after = await SpentIn(budget, period, ct);
            var before = after - Money.OfMinorUnits(body.AmountMinorUnits, budget.Currency);

            // The crossing: at or under the limit before, over it after. A single expense that lands
            // exactly on the limit has not exceeded it.
            if (before > budget.Amount || after <= budget.Amount)
                continue;

            drafts.Add(Draft(budget, period.Name, after));
        }

        return drafts;
    }

    /// <summary>
    /// Everything spent in this budget's category and currency during the period, including the
    /// expense that prompted this -- it is committed by the time the event is processed.
    /// </summary>
    private async Task<Money> SpentIn(Budget budget, BudgetPeriod period, CancellationToken ct)
    {
        var minorUnits = await db.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.SourceAccountId != null
                                  && transaction.DestinationAccountId == null
                                  && transaction.CategoryId == budget.CategoryId
                                  && transaction.Currency == budget.Currency
                                  && transaction.OccurredAt >= period.From
                                  && transaction.OccurredAt < period.ToExclusive)
            .SumAsync(transaction => transaction.AmountMinorUnits, ct);

        return Money.OfMinorUnits(minorUnits, budget.Currency);
    }

    private static NotificationDraft Draft(Budget budget, string period, Money spent)
    {
        var over = spent - budget.Amount;
        var label = budget.Category!.Label;

        return new NotificationDraft(
            NotificationKind.BudgetExceeded,
            $"{label} is over budget",
            $"You have spent {spent} of your {budget.Amount} budget for {label} in {period}, "
            + $"which is {over} over.",
            new BudgetExceededPayload(
                budget.Id,
                budget.CategoryId,
                label,
                period,
                budget.AmountMinorUnits,
                spent.MinorUnits,
                over.MinorUnits,
                budget.Currency.ToString()));
    }

    /// <summary>
    /// The facts, for a client that wants to link to the budget or show its own wording. Declared
    /// beside the rule that produces it, because nothing else has any business reading it.
    /// <para>
    /// Nothing time-varying in here: this is hashed to deduplicate, so a timestamp would make every
    /// redelivery look like a new notification.
    /// </para>
    /// </summary>
    private sealed record BudgetExceededPayload(
        int BudgetId,
        int CategoryId,
        string CategoryLabel,
        string Period,
        long LimitMinorUnits,
        long SpentMinorUnits,
        long ExceededByMinorUnits,
        string Currency);
}
