using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using Xunit.Abstractions;

namespace Xpense.Adapters.Postgres.Tests
{
    public abstract class IntegrationTestSuite(ITestOutputHelper output) : IAsyncLifetime
    {
        private const string ConnectionString =
            "Server=localhost;Port=4321;Database=devxpense;User Id=postgres;Password=password";

        private NpgsqlDataSource _dataSource = null!;

        private async Task<NpgsqlConnection> InitializeConnection()
        {
            var loggerFactor = LoggerFactory.Create(b => b
                .SetMinimumLevel(LogLevel.Information)
                .AddProvider(new XUnitLoggerProvider(output))
            );
            
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString)
                .UseLoggerFactory(loggerFactor)
                .EnableParameterLogging();
            _dataSource = dataSourceBuilder.Build();
            return await _dataSource.OpenConnectionAsync();
        }

        protected IDbConnection Connection { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Connection = await InitializeConnection();
            await TruncateTable();
            Construct();
        }

        public async Task DisposeAsync()
        {
            await TruncateTable();
            await _dataSource.DisposeAsync();
        }

        protected abstract Task TruncateTable();
        protected abstract void Construct();
    }
}