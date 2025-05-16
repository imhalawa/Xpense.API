using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Exceptions;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Transactions.UseCases
{
    public class GetAllTransactionsForAccountNumberUseCase(
            ITransactionRepository repository,
            IAccountRepository accountRepository
        ) : IQueryParamHandler<string, IEnumerable<Transaction>>
    {
        public async Task<IEnumerable<Transaction>> Execute(string accountNumber, CancellationToken cancellationToken = default)
        {
            //if (!string.IsNullOrWhiteSpace(accountNumber) && !await accountRepository.Exists(accountNumber))
            //    throw new AccountNotFoundException(accountNumber);
            //var account = await accountRepository.GetAccountByNumber(accountNumber);
            //return await repository.GetAllTransactions(account);
            throw new NotImplementedException();

        }
    }
}
