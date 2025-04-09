using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Transactions.UseCases
{
    public class GetAllTransactionsUseCase(ITransactionRepository repository) : IQueryHandler<IEnumerable<Transaction>>
    {
        public async Task<IEnumerable<Transaction>> Execute()
        {
            return await repository.GetAllTransactions();
        }
    }
}
