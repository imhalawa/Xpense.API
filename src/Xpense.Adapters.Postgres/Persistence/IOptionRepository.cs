
using Xpense.Adapters.Postgres.Models;

namespace Xpense.Adapters.Postgres.Persistence
{
    public interface IOptionRepository<T>
    {
        Task<StorageResult<T>> GetByLabel(string label, bool includeDeleted = false);
        Task<SimpleStorageResult> Restore(string label);
        Task<StorageResult<T?>> GetOrCreateIfMissing(T tag);
    }
}
