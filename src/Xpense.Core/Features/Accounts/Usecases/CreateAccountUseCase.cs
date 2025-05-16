using Xpense.Core.Exceptions;
using Xpense.Core.Features.Accounts.Commands;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Accounts.Usecases;

public class CreateAccountUseCase(IAccountRepository repository) : ICommandResultHandler<CreateAccountCommand, Account>
{
    public async Task<Account> Handle(CreateAccountCommand request)
    {
        var accountNumber = await repository.GetNextAccountNumber();
        var account = new Account()
        {
            Name = request.Name,
            Balance = request.Balance,
            AccountNumber = accountNumber.Data,
            IsDefaultAccount = !repository.HasDefaultAccount(),
        };

        repository.Create(account);
        var result = await repository.SaveChanges();

        if (result < 1)
        {
            throw new AccountCreationFailedException(request.Name);
        }

        return account;
    }
}