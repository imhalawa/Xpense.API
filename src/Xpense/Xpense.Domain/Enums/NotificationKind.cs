namespace Xpense.Domain.Enums;

/// <summary>
/// What class of thing a notification tells you about.
/// <para>
/// Stored as an integer, so values are only ever appended -- inserting one in the middle would
/// change what every existing row means.
/// </para>
/// </summary>
public enum NotificationKind
{
    /// <summary>Spending passed a budget's limit for the first time in its period.</summary>
    BudgetExceeded = 1
}
