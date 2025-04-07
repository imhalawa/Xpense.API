using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Entities;

namespace Xpense.Core.Features.Accounts.Usecases;

public class GetAccountByNumberUseCase(IAccountRepository repository) : IQueryParamHandler<string, Account>
{
    public async Task<Account> Execute(string accountNumber, CancellationToken cancellationToken = default)
    {
        var account = await repository.GetAccountByNumber(accountNumber);
        return account;
    }
}