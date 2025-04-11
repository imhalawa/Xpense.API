using Xpense.Adapters.Postgres.Postgres;

var databaseInitializer = new DatabaseInitializer("Server=localhost;Port=5432;Database=devxpense;User Id=postgres;Password=password");
databaseInitializer.Initialize();