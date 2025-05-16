using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Xpense.Persistence.Repositories;
using Xpense.Core.Features.Accounts.Usecases;
using Xpense.Core.Features.Analytics.UseCases;
using Xpense.Core.Features.Categories.UseCases;
using Xpense.Core.Features.Merchants.UseCases;
using Xpense.Core.Features.Tags.UseCases;
using Xpense.Core.Features.Transactions.UseCases;
using Xpense.Adapters.Postgres.Postgres;
using Xpense.Core.Interfaces.Persistence;
using Xpense.Adapters.Postgres.Repositories;


namespace Xpense.RestApi.Extensions.cs
{
    public static class IoC
    {
        public static void ConfigurePersistence(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddSingleton(_ => new DatabaseInitializer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Version = "v1",
                        Title = "Xpense",
                        Description = "Financial Tracking Services and Advisory",
                        // TODO: Add Terms of Use
                        Contact = new OpenApiContact()
                        {
                            Name = "Mohamed Halawa",
                            Email = "imhalawa@outlook.com",
                            Url = new Uri("https//www.traceintime.com")
                        },
                    }
                );

                // Read XML Comments Generated Document
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        public static void ConfigureApiVersioning(this IServiceCollection services)
        {
            // Register API Versioning Services, see https://github.com/dotnet/aspnet-api-versioning/wiki
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IPriorityRepository, PriorityRepository>();
        }

        public static void AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<GetAccountByNumberUseCase>();
            services.AddScoped<GetAllAccountsUseCase>();
            services.AddScoped<CreateAccountUseCase>();
            services.AddScoped<DeleteAccountUseCase>();
            services.AddScoped<UpdateAccountUseCase>();

            services.AddScoped<CreateCategoryUseCase>();
            services.AddScoped<GetAllCategoriesUseCase>();
            services.AddScoped<GetCategoryByIdUseCase>();
            services.AddScoped<DeleteCategoryByIdUseCase>();
            services.AddScoped<UpdateCategoryUseCase>();

            services.AddScoped<CreateTagUseCase>();
            services.AddScoped<DeleteTagUseCase>();
            services.AddScoped<GetTagByIdUseCase>();
            services.AddScoped<GetAllTagsUseCase>();
            services.AddScoped<UpdateTagUseCase>();

            services.AddScoped<GetAllMerchantsUseCase>();

            services.AddScoped<DepositTransactionUseCase>();
            services.AddScoped<WithdrawTransactionUseCase>();
            services.AddScoped<GetAllTransactionsForAccountNumberUseCase>();
            services.AddScoped<GetAllTransactionsUseCase>();
            services.AddScoped<FilterTransactionsUseCase>();

            services.AddScoped<GetExpensesByCategoryUseCase>();
        }
    }
}
