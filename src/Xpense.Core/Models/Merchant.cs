using Xpense.Core.Abstract.Entities;

namespace Xpense.Core.Models
{
    public class Merchant : BaseEntity, IOptionEntity
    {
        public required string Label { get; set; }
        public IEnumerable<Transaction>? Transactions { get; set; }
    }
}
