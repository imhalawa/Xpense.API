using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;
using Xpense.Core.Features.Accounts.Commands;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Accounts.Usecases;

public class CreateAccountUseCase(IAccountRepository repository) : ICommandResultHandler<CreateAccountCommand, Account>
{
    public async Task<Account> Handle(CreateAccountCommand request)
    {
        var account = new Account()
        {
            Name = request.Name,
            Balance = request.Balance,
            AccountNumber = repository.GetNextAccountNumber(),
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