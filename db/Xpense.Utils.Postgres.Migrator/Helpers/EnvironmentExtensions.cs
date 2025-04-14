using Xpense.Utils.Postgres.Migrator.Exceptions;

namespace Xpense.Utils.Postgres.Migrator.Helpers
{
    public static class EnvironmentExtensions
    {
        public static HostingEnvironment TryGetEnvironment()
        {
            const string variable = "DOTNET_ENVIRONMENT";
            var environment = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrWhiteSpace(environment))
            {
                return HostingEnvironment.Production;
            }

            return environment.ToLower() switch
            {
                "development" => HostingEnvironment.Development,
                "docker" => HostingEnvironment.Docker,
                "production" => HostingEnvironment.Production,
                _ => throw new UnknownHostingEnvironment(environment),
            };
        }

    }
}
