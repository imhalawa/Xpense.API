using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Entities;

namespace Xpense.Core.Features.Accounts.Usecases;

public class GetAllAccountsUseCase(IAccountRepository repository): IQueryHandler<IEnumerable<Account>>
{
    public async Task<IEnumerable<Account>> Execute()
    {
        var result = await repository.GetAll();
        return result;
    }
}