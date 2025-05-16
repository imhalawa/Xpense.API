using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Accounts.Usecases;

public class GetAccountByNumberUseCase(IAccountRepository repository) : IQueryParamHandler<string, Account>
{
    public async Task<Account> Execute(string accountNumber, CancellationToken cancellationToken = default)
    {
        var account = await repository.GetAccountByNumber(accountNumber);
        return account;
    }
}