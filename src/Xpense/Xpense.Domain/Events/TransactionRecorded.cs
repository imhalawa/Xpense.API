using Xpense.Domain.Enums;

namespace Xpense.Domain.Events;

/// <summary>
/// A transaction was recorded. Says what happened and nothing about whether anyone should be told --
/// that judgement belongs to whatever consumes this. See
/// docs/adr/0006-a-budget-reports-and-never-blocks.md.
/// <para>
/// Carries enough for every rule to reach a verdict without re-reading the transaction, which also
/// means a rule cannot be affected by anything that changed after the fact. The balances are the
/// balances *after* this movement, because that is what was true when it happened.
/// </para>
/// <para>
/// Primitives and ids only. No <c>Transaction</c>, no <c>Money</c>: this is a wire format that
/// outlives the request that wrote it.
/// </para>
/// </summary>
/// <param name="Kind">Derived on the entity, stated here, because a rule filtering on it should not re-derive it.</param>
/// <param name="CategoryId">Null on a transfer, which has no spending class.</param>
/// <param name="MerchantId">Null on a transfer, which has no counterparty outside Xpense.</param>
/// <param name="SourceAccountNumber">Null on income, where the money came from outside Xpense.</param>
/// <param name="SourceBalanceAfterMinorUnits">The source account's balance once this was applied.</param>
/// <param name="DestinationAccountNumber">Null on an expense, where the money left Xpense.</param>
/// <param name="DestinationBalanceAfterMinorUnits">The destination account's balance once this was applied.</param>
public sealed record TransactionRecorded(
    int TransactionId,
    TransactionKind Kind,
    long AmountMinorUnits,
    Currency Currency,
    DateTime OccurredAt,
    int? CategoryId,
    int? MerchantId,
    string? SourceAccountNumber,
    long? SourceBalanceAfterMinorUnits,
    string? DestinationAccountNumber,
    long? DestinationBalanceAfterMinorUnits) : EventBody;
