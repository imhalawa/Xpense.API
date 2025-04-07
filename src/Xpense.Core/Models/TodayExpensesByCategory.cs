using Xpense.Core.ValueObjects;

namespace Xpense.Core.Models
{
    public class TodayExpensesByCategory(IEnumerable<ExpensesByCategory>? expenses, Money total)
    {
        public IEnumerable<ExpensesByCategory>? Expenses { get; set; } = expenses;
        public Money Total { get; set; } = total;
    }
}
