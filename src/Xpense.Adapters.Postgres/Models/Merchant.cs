namespace Xpense.Adapters.Postgres.Models
{
    public class Merchant : BaseEntity, IOptionEntity
    {
        public required string Label { get; set; }
        public IEnumerable<Adapters.Postgres.Models.Transaction>? Transactions { get; set; }
    }
}
