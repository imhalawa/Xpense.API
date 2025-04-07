using Xpense.Core.Features.Transactions.Commands;
using Xpense.Core.Models;
using Xpense.Core.ValueObjects;

namespace Xpense.RestApi.Models;

public class DepositTransactionRequest
{
    public Money Amount { get; set; }
    public long? CreatedOn { get; set; }
    public string AccountNumber { get; set; }
    public int CategoryId { get; set; }
    public Merchant Merchant { get; set; }
    public Tag[] Tags { get; set; }

    public DepositTransactionCommand ToCommand() => new(Amount, AccountNumber, CategoryId, Merchant, Tags, CreatedOn);
}