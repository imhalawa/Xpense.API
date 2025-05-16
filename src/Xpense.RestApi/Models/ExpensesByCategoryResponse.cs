using Xpense.Adapters.Postgres;
using Xpense.Core.Models;
using Xpense.Core.ValueObjects;

namespace Xpense.RestApi.Models
{
    public class ExpensesByCategoryResponse(int id, CategoryResponse category, Money amount)
    {
        public int Id { get; set; } = id;
        public CategoryResponse Category { get; set; } = category;
        public Money Amount { get; set; } = amount;

        public static ExpensesByCategoryResponse Of(ExpensesByCategory expensesByCategory) => new ExpensesByCategoryResponse(expensesByCategory.Id, CategoryResponse.Of(expensesByCategory.Category), expensesByCategory.Amount);
    }
}
