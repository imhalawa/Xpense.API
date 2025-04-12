using System.Reflection;
using DbUp;
using Xpense.Postgres.Exceptions;

namespace Xpense.Postgres;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        
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
            throw new DatabaseInitializationException();
    }
}