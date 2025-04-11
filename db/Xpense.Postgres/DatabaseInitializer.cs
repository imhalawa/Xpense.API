using System.Reflection;
using DbUp;

namespace Xpense.Adapters.Postgres.Postgres;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
    }

    public void Initialize()
    {
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);
        var upgrader = DeployChanges.To.PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception("Unable to apply db migrations"); // TODO: Custom Exception, Better message
        }
    }
}