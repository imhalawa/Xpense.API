namespace Xpense.Adapters.Postgres.Models
{
    public class ExpensesByCategory(int id, Category category, Money amount)
    {
        public int Id { get; set; } = id;
        public Category Category { get; set; } = category;
        public Money Amount { get; set; } = amount;
    }
}
