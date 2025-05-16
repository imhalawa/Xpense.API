using Xpense.Adapters.Postgres.Models;

namespace Xpense.Adapters.Postgres.Persistence;

public interface IAccountRepository
{
    Task<StorageResult<Account>> Create(Account account);
    Task<SimpleStorageResult> DeleteById(int accountId);
    Task<SimpleStorageResult> DeleteByAccountNumber(string accountNumber);
    Task<SimpleStorageResult> Exists(int accountId);
    Task<StorageResult<Account>> GetById(int accountId, bool excludeDeleted = true);
    Task<StorageResult<Account>> GetByAccountNumber(string accountNumber, bool excludeDeleted = true);
    Task<StorageResult<Account>> GetDefaultAccount(bool excludeDeleted = true);
    Task<StorageResult<string>> GetNextAccountNumber();
    Task<bool> HasDefaultAccount();
    Task<bool> IsDeleted(int accountId);
    Task<SimpleStorageResult> Restore(int accountId);
}
