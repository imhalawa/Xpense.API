﻿using Xpense.Services.Abstract.Persistence;
using Xpense.Services.Abstract.UseCases;
using Xpense.Services.Entities;
using Xpense.Services.Exceptions;

namespace Xpense.Services.Features.Transactions.UseCases
{
    public class GetAllTransactionsForAccountNumberUseCase(
            ITransactionRepository repository,
            IAccountRepository accountRepository
        ) : IQueryParamHandler<string, IEnumerable<Transaction>>
    {
        public async Task<IEnumerable<Transaction>> Execute(string accountNumber, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(accountNumber) && !await accountRepository.Exists(accountNumber))
                throw new AccountNotFoundException(accountNumber);
            var account = await accountRepository.GetAccountByNumber(accountNumber);
            return await repository.GetAllTransactions(account);
        }
    }
}
