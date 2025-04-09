using Xpense.Core.Models;
using Xpense.Core.ValueObjects;

namespace Xpense.Core.Features.Transactions.Commands;

public class WithdrawTransactionCommand(Money amount, string accountNumber, int category, MerchantOption merchantOption, TagOption[]? tags = null, long? createdOn = null)
{
    public Money Amount { get; set; } = amount;
    public long? CreatedOn { get; set; } = createdOn;
    public string AccountNumber { get; set; } = accountNumber;
    public int CategoryId { get; set; } = category;
    public MerchantOption merchantOption { get; set; } = merchantOption;
    public TagOption[]? Tags { get; set; } = tags;
}