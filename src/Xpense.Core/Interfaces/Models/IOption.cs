using Xpense.Core.Interfaces.Entities;

namespace Xpense.Core.Interfaces.Models
{
    public interface IOption<T> where T : IOptionEntity
    {
        int? Id { get; set; }
        string Label { get; set; }
        bool Create { get; set; }

        T ToEntity();
    }
}
