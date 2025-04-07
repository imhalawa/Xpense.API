using Xpense.Core.Abstract.Entities;
using Xpense.Core.Abstract.Models;

namespace Xpense.Core.Abstract.Persistence
{
    public interface IOptionRepository<T> : IRepository<T> where T : BaseEntity, IOptionEntity
    {
        Task<T?> GetByLabel(string Label, bool ignoreFilters = false);
        bool TryRestore(string Label, out T? result);
        Task<T?> GetOrCreateIfMissing<K>(K model) where K : IOption<T>;
    }
}
