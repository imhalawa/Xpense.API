using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Enums;
using Xpense.Core.Exceptions;
using Xpense.Core.Features.Transactions.Commands;
using Xpense.Core.Helpers;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Transactions.UseCases;

public class DepositTransactionUseCase(
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ITagRepository tagRepository,
    ITransactionRepository transactionRepository,
    IMerchantRepository merchantRepository
) : ICommandResultHandler<DepositTransactionCommand, Transaction>
{
    public async Task<Transaction> Handle(DepositTransactionCommand command)
    {
        //if (!string.IsNullOrWhiteSpace(command.AccountNumber) && !await accountRepository.Exists(command.AccountNumber))
        //    throw new AccountNotFoundException(command.AccountNumber);

        //var category = await categoryRepository.GetWithById(command.CategoryId, s => s.Priority) ?? throw new CategoryNotFoundException(command.CategoryId);

        //var account = string.IsNullOrWhiteSpace(command.AccountNumber)
        //                   ? await accountRepository.GetDefaultAccount()
        //                   : await accountRepository.GetAccountByNumber(command.AccountNumber);

        //account.Deposit(command.Amount.ToSingle());

        //var merchant = await merchantRepository.GetOrCreateIfMissing(command.merchantOption) ?? throw new MerchantNotFoundException(command.merchantOption.Label);

        //var tags = command.Tags != null
        //    ? await command.Tags.ToAsyncEnumerable()
        //        .SelectAwait(async t => await tagRepository.GetOrCreateIfMissing(t))
        //        .Where(t => t != null).ToListAsync()
        //    : null;

        //var transaction = new Transaction
        //{
        //    Amount = command.Amount.Cents,
        //    Currency = command.Amount.Currency,
        //    Category = category,
        //    Account = account,
        //    CreatedOn = command.CreatedOn.ToDateTime() ?? DateTime.Now,
        //    Tags = tags,
        //    Merchant = merchant,
        //    TransactionType = TransactionType.Credit
        //};

        //transactionRepository.Create(transaction);
        //var result = await transactionRepository.SaveChanges();

        //if (result < 1)
        //    throw new DepositCreationFailedException(command.Amount.ToSingle(), command.AccountNumber);

        //return transaction;
        throw new NotImplementedException();

    }
}