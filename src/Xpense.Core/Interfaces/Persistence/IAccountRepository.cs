using Xpense.Core.Models;

namespace Xpense.Core.Interfaces.Persistence;

public interface IAccountRepository
{
    Task<SimpleStorageResult> Create(Account account);
    Task<SimpleStorageResult> Delete(int accountId);
    Task<SimpleStorageResult> DeleteByAccountNumber(string accountNumber);
    Task<SimpleStorageResult> Exists(int accountId);
    Task<StorageResult<Account>> GetAccountByNumber(string accountNumber);
    Task<StorageResult<Account>> GetDefaultAccount();
    Task<StorageResult<string>> GetNextAccountNumber();
    Task<SimpleStorageResult> HasDefaultAccount();
    Task<SimpleStorageResult> IsDeleted(int accountId);
    Task<SimpleStorageResult> Restore(int accountId);
    Task<SimpleStorageResult> TryRestore(int accountId, out Account? result);
}
