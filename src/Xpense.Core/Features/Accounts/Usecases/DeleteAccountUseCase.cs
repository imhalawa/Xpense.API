using Xpense.Core.Exceptions;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;

namespace Xpense.Core.Features.Accounts.Usecases;

public class DeleteAccountUseCase(IAccountRepository repository): ICommandHandler<string>
{
    public async Task Handle(string accountNumber)
    {
       repository.DeleteAccountByNumber(accountNumber);
       var result = await repository.SaveChanges();
       if (result < 1)
       {
           throw new AccountUpdateFailedException(accountNumber);
       }
    }
}