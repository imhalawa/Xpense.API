using Xpense.Core.Abstract.Entities;

namespace Xpense.Core.Models;

public class Tag : BaseEntity, IOptionEntity
{
    public required string Label { get; set; }
    public string? BgColorHex { get; set; }
    public string? FgColorHex { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; }
}