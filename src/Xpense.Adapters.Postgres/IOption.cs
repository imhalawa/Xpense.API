using Xpense.Adapters.Postgres.Models;

namespace Xpense.Adapters.Postgres
{
    public interface IOption<T> where T : IOptionEntity
    {
        int? Id { get; set; }
        string Label { get; set; }
        bool Create { get; set; }

        T ToEntity();
    }
}
