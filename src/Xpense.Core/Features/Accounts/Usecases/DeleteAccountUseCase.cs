using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Exceptions;

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