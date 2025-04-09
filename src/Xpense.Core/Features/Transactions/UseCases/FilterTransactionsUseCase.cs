using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Transactions.UseCases
{
    public class FilterTransactionsUseCase(ITransactionRepository repository) : IQueryParamHandler<FilterQuery, PaginatedResult<Transaction>>
    {
        public async Task<PaginatedResult<Transaction>> Execute(FilterQuery query, CancellationToken cancellationToken = default)
        {
            if (!query.IsValid())
                throw new InvalidFilteredResultParams(query);

            var result = await repository.Filter(query.Page, query.Size, query.date);
            return result;
        }
    }
}
