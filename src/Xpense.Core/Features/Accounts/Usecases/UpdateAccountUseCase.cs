using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Entities;
using Xpense.Core.Exceptions;
using Xpense.Core.Features.Accounts.Commands;

namespace Xpense.Core.Features.Accounts.Usecases;

public class UpdateAccountUseCase(IAccountRepository repository)
    : ICommandResultHandler<UpdateAccountCommand, Account>
{
    public async Task<Account> Handle(UpdateAccountCommand command)
    {
        var account = await repository.GetAccountByNumber(command.Number);
        account.Name = command.Name;
        account.IsDefaultAccount = command.IsDefault;
        repository.Update(account);
        var result = await repository.SaveChanges();
        if (result < 1)
            throw new AccountUpdateFailedException(command.Number);
        return account;
    }
}