using Xpense.Core.Entities;

namespace Xpense.Core.Abstract.Persistence;

public interface IAccountRepository : IRepository<Account>
{
    string GetNextAccountNumber();
    bool HasDefaultAccount();
    void DeleteAccountByNumber(string accountNumber);
    Task<Account> GetAccountByNumber(string accountNumber);
    Task<Account> GetDefaultAccount();
    Task<bool> Exists(string accountNumber);
}
