namespace Xpense.Adapters.Postgres.Exceptions;

public class XpenseException(string message, Exception? innerException = null) : Exception(message, innerException);