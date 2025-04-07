using Xpense.Core.Abstract.Entities;

namespace Xpense.Core.Entities
{
    public class Merchant : BaseEntity, IOptionEntity
    {
        public required string Label { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
