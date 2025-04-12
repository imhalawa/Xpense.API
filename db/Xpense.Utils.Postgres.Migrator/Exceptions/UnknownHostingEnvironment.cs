namespace Xpense.Utils.Postgres.Migrator.Exceptions
{
    public class UnknownHostingEnvironment : Exception
    {
        public UnknownHostingEnvironment(string environment) : base($"Please ensure a correct environment set in `DOTNET_ENVIRONMENT`, found: {environment}")
        {
        }
    }
}
