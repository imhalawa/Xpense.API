using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Xpense.Notifications;
using Xpense.Notifications.Rules;
using Xpense.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// The same connection string variable the API uses. Nothing ships a default, so a missing one fails
// at startup rather than quietly reaching localhost.
builder.Services.AddDbContext<XpenseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Rules are discovered, not listed. Adding one is adding a file.
builder.Services.AddNotificationRules();

builder.Services.AddScoped<EventProcessor>();
builder.Services.AddHostedService<EventPump>();

// No migrations here either: the schema is applied by the migrations container before anything starts.
// See docs/adr/0004-migrations-are-a-deployment-step.md.
await builder.Build().RunAsync();
