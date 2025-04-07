using Xpense.Core.Features.Accounts.Commands;

namespace Xpense.RestApi.Models;

public class UpdateAccountRequest(string number, string name, bool isDefault)
{
    public string Number { get; set; } = number;
    public string Name { get; set; } = name;
    public bool IsDefault { get; set; } = isDefault;

    public UpdateAccountCommand ToCommand() => new(Number, Name, IsDefault);
}