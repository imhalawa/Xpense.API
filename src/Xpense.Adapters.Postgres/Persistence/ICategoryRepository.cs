using Xpense.Adapters.Postgres.Models;

namespace Xpense.Adapters.Postgres.Persistence;

public interface ICategoryRepository
{
    Task<StorageResult<Category>> Create(Category category);
    Task<StorageResult<IEnumerable<Category>?>> Get(bool includeDeleted = false);
    Task<StorageResult<Category?>> GetById(int categoryId, bool includeDeleted = false);
    Task<SimpleStorageResult> DeleteById(int categoryId);
    Task<SimpleStorageResult> Restore(int categoryId);
    Task<SimpleStorageResult> IsDeleted(int categoryId);
    Task<SimpleStorageResult> Exists(int categoryId);
}