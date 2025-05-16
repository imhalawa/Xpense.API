using Xpense.Core.Models;

namespace Xpense.Core.Interfaces.Persistence;

public interface ITransactionRepository 
{
    Task<IEnumerable<Transaction>> GetAllTransactions(Account account);
    Task<IReadOnlyCollection<Transaction>> GetAllTransactions(long? date = null);
    Task<PaginatedResult<Transaction>> Filter(int page, int pageSize, long? date = null);
}