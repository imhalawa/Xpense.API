using Microsoft.Extensions.Configuration;
using Xpense.Postgres;
using Xpense.Utils.Postgres.Migrator.Helpers;

var environment = EnvironmentExtensions.TryGetEnvironment();

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment.ToString().ToLower()}.json", true, true)
    .Build();

var databaseInitializer = new DatabaseInitializer(configuration.GetConnectionString("DefaultConnection"));
databaseInitializer.Initialize();