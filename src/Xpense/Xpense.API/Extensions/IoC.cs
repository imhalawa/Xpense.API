using FluentValidation;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System;
using System.IO;
using System.Reflection;
using Xpense.API.ExceptionHandlers;
using Xpense.API.Infrastructure;
using Xpense.Domain.Events;
using Xpense.Persistence;

namespace Xpense.API.Extensions;

public static class IoC
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XpenseDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly("Xpense.Persistence"));
        });
    }

    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(OptionResolver<>));

        // Emitting an event is inserting a row through the caller's DbContext, so this is scoped like
        // the context it writes to. Nothing here connects to a broker, because there is not one --
        // see docs/adr/0008-the-events-table-is-the-queue.md.
        services.AddScoped<IEventBus, EventBus>();
    }

    public static void AddRequestValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(IEndpoint).Assembly);
    }

    public static void AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<InsufficientFundsExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<DomainRuleViolationExceptionHandler>();
        services.AddExceptionHandler<PersistenceFailedExceptionHandler>();
        services.AddExceptionHandler<FallbackExceptionHandler>();
    }

    public static void AddHealthProbe(this IServiceCollection services)
    {
        services.AddHealthChecks().AddDbContextCheck<XpenseDbContext>();
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Slices nest their request as `Request`, so by default every one of them claims the schema
            // id "Request" and generation fails outright with a collision -- which is why
            // /swagger/v1/swagger.json returned 500 rather than a document. Qualifying a nested type
            // with the names it is declared inside turns those into CreateBudgetRequest,
            // CreateAccountRequest and so on.
            options.CustomSchemaIds(SchemaId);
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Xpense.API",
                    Description = "Financial Tracker and advisor",
                    // TODO: Add Terms of Use
                    Contact = new OpenApiContact
                    {
                        Name = "Mohamed Halawa",
                        Email = "imhalawa@outlook.com",
                        Url = new Uri("https://www.halawa.dev")
                    },
                });

            // Read XML Comments Generated Document
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private static string SchemaId(Type type)
    {
        var names = new List<string>();

        for (var current = type; current is not null; current = current.DeclaringType)
            names.Insert(0, current.Name);

        // Generics arrive as `Thing`1`; the arity suffix is not valid in a schema id.
        return string.Concat(names).Replace("`", string.Empty);
    }
}
