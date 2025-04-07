
using Xpense.Core.Entities;
using Xpense.Core.Models;

namespace Xpense.Core.Abstract.Persistence;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetAllTransactions(Account account);
    Task<IReadOnlyCollection<Transaction>> GetAllTransactions(long? date = null);
    Task<PaginatedResult<Transaction>> Filter(int page, int pageSize, long? date = null);
}