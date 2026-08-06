using System.Globalization;
using Xpense.Domain.Enums;
using Xpense.Domain.Exceptions;
using Xpense.Domain.ValueObjects;

namespace Xpense.Domain.Entities;

/// <summary>
/// An intended limit on expense for one category, in one currency, over a period.
/// <para>
/// A budget reports and never blocks. Nothing consults it when a transaction is recorded, because
/// Xpense records money that has already moved -- refusing the record would not un-spend anything,
/// it would only make Xpense disagree with the bank. See
/// docs/adr/0006-a-budget-reports-and-never-blocks.md.
/// </para>
/// <para>
/// Budgets do not know about each other. Several may cover one category at once, in different
/// currencies, over different lengths, or over the very same days, and nothing here arbitrates
/// between them: <c>Spent</c> and <c>Remaining</c> belong to a budget, not to a category. See
/// docs/adr/0007-budgets-are-independent-of-one-another.md.
/// </para>
/// <para>
/// One entity covers both shapes. A one-off is a budget with no <see cref="Recurrence"/> and a
/// fixed window; a repeating one has a recurrence and may run indefinitely. A second entity
/// differing by one column is the mistake ADR 0001 already corrected once.
/// </para>
/// </summary>
public class Budget : BaseEntity
{
    /// <summary>The limit in minor units of <see cref="Currency"/>. Mapped; prefer <see cref="Amount"/>.</summary>
    public long AmountMinorUnits { get; set; }

    /// <summary>
    /// The currency this budget is stated in. Only expenses in this currency count toward it --
    /// nothing converts, here or anywhere else in Xpense.
    /// </summary>
    public Currency Currency { get; set; }

    /// <summary>The limit as money. Not mapped -- projected from the two columns above.</summary>
    public Money Amount => Money.OfMinorUnits(AmountMinorUnits, Currency);

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public Recurrence Recurrence { get; set; }

    /// <summary>The first day this budget is in force, as a UTC date.</summary>
    public DateTime StartsOn { get; set; }

    /// <summary>
    /// The last day this budget is in force, inclusive, as a UTC date. Null means it runs
    /// indefinitely, which only a repeating budget may do.
    /// </summary>
    public DateTime? EndsOn { get; set; }

    /// <summary>
    /// The share of <see cref="Amount"/> at which it is worth saying something, before the limit
    /// itself is reached. Null means say nothing early. Defaults to <see cref="DefaultAlertThreshold"/>.
    /// </summary>
    public int? AlertThresholdPercent { get; set; }

    /// <summary>Three quarters: late enough to be worth hearing, early enough to act on.</summary>
    public const int DefaultAlertThreshold = 75;

    /// <summary>
    /// The amount at which this budget's alert threshold is reached, or null when it has none.
    /// <para>
    /// Rounded down, so the threshold is crossed at the first minor unit that genuinely reaches the
    /// share rather than a fraction before it.
    /// </para>
    /// </summary>
    public Money? AlertThreshold =>
        AlertThresholdPercent is null
            ? null
            : Money.OfMinorUnits(AmountMinorUnits * AlertThresholdPercent.Value / 100, Currency);

    /// <summary>The first instant after this budget's life. Open-ended budgets never reach it.</summary>
    private DateTime LifeToExclusive => EndsOn?.AddDays(1) ?? DateTime.MaxValue;

    public static Budget For(
        Category category,
        Money amount,
        Recurrence recurrence,
        DateTime startsOn,
        DateTime? endsOn,
        int? alertThresholdPercent = DefaultAlertThreshold)
    {
        if (amount.MinorUnits <= 0)
            throw new InvalidBudgetException("A budget amount must be positive.");

        var (from, to) = RequireValidWindow(recurrence, startsOn, endsOn);

        return new Budget
        {
            Category = category,
            AmountMinorUnits = amount.MinorUnits,
            Currency = amount.Currency,
            Recurrence = recurrence,
            StartsOn = from,
            EndsOn = to,
            AlertThresholdPercent = RequireValidThreshold(alertThresholdPercent),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Restates an existing budget's limit and window. The category is not among them: a budget for
    /// a different category is a different budget, the same way an account's currency is fixed for
    /// its life.
    /// </summary>
    public void Restate(
        Money amount,
        Recurrence recurrence,
        DateTime startsOn,
        DateTime? endsOn,
        int? alertThresholdPercent = DefaultAlertThreshold)
    {
        if (amount.MinorUnits <= 0)
            throw new InvalidBudgetException("A budget amount must be positive.");

        var (from, to) = RequireValidWindow(recurrence, startsOn, endsOn);
        var threshold = RequireValidThreshold(alertThresholdPercent);

        AmountMinorUnits = amount.MinorUnits;
        Currency = amount.Currency;
        Recurrence = recurrence;
        StartsOn = from;
        EndsOn = to;
        AlertThresholdPercent = threshold;
        Touch();
    }

    /// <summary>
    /// A threshold is a share of the limit, so it lies between 1 and 100. Null is allowed and means
    /// this budget says nothing before the limit is passed.
    /// <para>
    /// 100 is permitted and is not the same as saying nothing: it fires on reaching the limit exactly,
    /// where "exceeded" needs the limit passed.
    /// </para>
    /// </summary>
    private static int? RequireValidThreshold(int? percent)
    {
        if (percent is not null && percent is < 1 or > 100)
            throw new InvalidBudgetException("An alert threshold must be between 1 and 100 percent.");

        return percent;
    }

    private static (DateTime From, DateTime? To) RequireValidWindow(
        Recurrence recurrence,
        DateTime startsOn,
        DateTime? endsOn)
    {
        var from = UtcDate(startsOn);
        var to = endsOn is null ? null : (DateTime?)UtcDate(endsOn.Value);

        // A budget that never repeats has exactly one window, so it has to say where that window
        // stops. Open-ended only means anything for a repeating one.
        if (recurrence == Recurrence.None && to is null)
            throw new InvalidBudgetException("A budget that does not repeat must state when it ends.");

        if (to is not null && to < from)
            throw new InvalidBudgetException("A budget cannot end before it starts.");

        return (from, to);
    }

    /// <summary>
    /// The period this budget measures at the given instant, or null when it measures nothing then.
    /// </summary>
    public BudgetPeriod? PeriodOn(DateTime instant)
    {
        if (Recurrence == Recurrence.None)
        {
            var only = OnlyPeriod();
            return only.Contains(instant) ? only : null;
        }

        var period = CalendarPeriodContaining(instant);

        // Whether a repeating budget is measuring is decided by the period overlapping its life,
        // not by the instant being inside that life. A monthly budget starting on the 15th measures
        // the whole of that month -- so asking about the 3rd has to give the same answer as asking
        // about the 20th, or the same period would report two different totals.
        return period.Overlaps(StartsOn, LifeToExclusive) ? period : null;
    }

    private BudgetPeriod OnlyPeriod() =>
        new(StartsOn, LifeToExclusive, $"{Day(StartsOn)}..{Day(EndsOn!.Value)}");

    private BudgetPeriod CalendarPeriodContaining(DateTime instant) => Recurrence switch
    {
        Recurrence.Weekly => Week(instant),
        Recurrence.Monthly => Month(instant),
        Recurrence.Yearly => Year(instant),
        _ => throw new InvalidBudgetException($"Unsupported recurrence {Recurrence}.")
    };

    /// <summary>
    /// ISO 8601 weeks: Monday to Sunday, and week 1 is the one holding the first Thursday.
    /// <para>
    /// The year in the name comes from <see cref="ISOWeek.GetYear"/> and not from the instant,
    /// because they disagree at the boundary -- 2027-01-01 falls in week 53 of 2026, and naming it
    /// 2027-W53 would name a week that does not exist.
    /// </para>
    /// </summary>
    private static BudgetPeriod Week(DateTime instant)
    {
        var weekYear = ISOWeek.GetYear(instant);
        var week = ISOWeek.GetWeekOfYear(instant);
        var monday = DateTime.SpecifyKind(ISOWeek.ToDateTime(weekYear, week, DayOfWeek.Monday), DateTimeKind.Utc);

        return new BudgetPeriod(monday, monday.AddDays(7), $"{weekYear:0000}-W{week:00}");
    }

    private static BudgetPeriod Month(DateTime instant)
    {
        var first = new DateTime(instant.Year, instant.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return new BudgetPeriod(first, first.AddMonths(1), $"{instant.Year:0000}-{instant.Month:00}");
    }

    private static BudgetPeriod Year(DateTime instant)
    {
        var first = new DateTime(instant.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return new BudgetPeriod(first, first.AddYears(1), $"{instant.Year:0000}");
    }

    private static DateTime UtcDate(DateTime value) => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc);

    private static string Day(DateTime value) => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
