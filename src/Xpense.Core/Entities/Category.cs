using Xpense.Core.Abstract.Entities;

namespace Xpense.Core.Entities;

public class Category : BaseEntity
{
    public string Label { get; set; }
    public int PriorityId { get; set; }
    public Priority Priority { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
}