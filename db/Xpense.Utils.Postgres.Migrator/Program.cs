using Microsoft.Extensions.Configuration;
using Xpense.Postgres;
using Xpense.Utils.Postgres.Migrator.Helpers;

var environment = EnvironmentExtensions.TryGetEnvironment();
Console.Write($"Current Environment: {environment}");
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment.ToString().ToLower()}.json", true, true)
    .Build();

var databaseInitializer = new DatabaseInitializer(
    configuration.GetConnectionString("DefaultConnection")
);

databaseInitializer.Initialize();