namespace Xpense.Core.Exceptions;

public class DepositCreationFailedException(decimal amount, string accountNumber, Exception? innerException = null)
    : XpenseException($"Failure Attempt to deposit amount {amount} to account {accountNumber}", innerException);

public class WithdrawCreationFailedException(decimal amount, string accountNumber, Exception? innerException = null)
    : XpenseException($"Failure Attempt to withdraw amount {amount} from account {accountNumber}", innerException);