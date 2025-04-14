using System.Reflection;
using DbUp;
using Xpense.Postgres.Exceptions;

namespace Xpense.Postgres;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string? connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);
        var upgrade = DeployChanges
            .To.PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransactionPerScript()
            .LogToConsole()
            .LogScriptOutput()
            .Build();

        var result = upgrade.PerformUpgrade();

        if (!result.Successful)
            throw new DatabaseInitializationException();
    }
}