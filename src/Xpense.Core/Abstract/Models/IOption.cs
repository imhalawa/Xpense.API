using Xpense.Core.Abstract.Entities;

namespace Xpense.Core.Abstract.Models
{
    public interface IOption<T> where T : IOptionEntity
    {
        int? Id { get; set; }
        string Label { get; set; }
        bool Create { get; set; }

        T ToEntity();
    }
}
