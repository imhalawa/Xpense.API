using Xpense.Core.Interfaces.Entities;
using Xpense.Core.Interfaces.Models;

namespace Xpense.Core.Abstract.Persistence
{
    public interface IOptionRepository<T>
    {
        //Task<T?> GetByLabel(string Label, bool ignoreFilters = false);
        //bool TryRestore(string Label, out T? result);
        //Task<T?> GetOrCreateIfMissing<K>(K model) where K : IOption<T>;
    }
}
