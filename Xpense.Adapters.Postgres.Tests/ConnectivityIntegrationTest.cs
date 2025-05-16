using Dapper;
using FluentAssertions;
using Xunit.Abstractions;

namespace Xpense.Adapters.Postgres.Tests
{
    public class ConnectivityIntegrationTest(ITestOutputHelper outputHelper): IntegrationTestSuite(outputHelper)
    {
        [Fact]
        public async Task Connectivity()
        {
            var result = await Connection.QueryFirstOrDefaultAsync<int?>("SELECT 1");
            result.Should().Be(1, because: "a basic query should succeed if the DB connection is valid");
        }

        protected override Task TruncateTable()
        {
            return Task.CompletedTask;
        }

        protected override void Construct()
        {
        }
    }
}
