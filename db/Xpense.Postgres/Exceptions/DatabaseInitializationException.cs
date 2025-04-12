namespace Xpense.Postgres.Exceptions
{
    public class DatabaseInitializationException(string message = "DbUp: unable to apply database migrations") : Exception(message);
}
