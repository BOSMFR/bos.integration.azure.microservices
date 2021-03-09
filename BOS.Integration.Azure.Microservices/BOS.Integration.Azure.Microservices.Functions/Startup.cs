using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.DataAccess.Repositories;
using BOS.Integration.Azure.Microservices.Functions.Extensions;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(BOS.Integration.Azure.Microservices.Functions.Startup))]
namespace BOS.Integration.Azure.Microservices.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfiguration configuration = builder.GetContext().Configuration;

            builder.Services.AddApplicationInsightsTelemetry(configuration["ApplicationInsights:InstrumentationKey"]);

            builder.Services.AddTransient<IConfigurationManager, ConfigurationManager>();
            builder.Services.AddTransient<IHttpService, HttpService>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<IValidationService, ValidationService>();
            builder.Services.AddTransient<IPrimeCargoService, PrimeCargoService>();
            builder.Services.AddTransient<IServiceBusService, ServiceBusService>();
            builder.Services.AddTransient<ILogService, LogService>();
            builder.Services.AddTransient<IBlobService, BlobService>();
            builder.Services.AddTransient<IShopService, ShopService>();

            CosmosDbSettings cosmosDbConfig = configuration.GetSection("CosmosDbConfig").Get<CosmosDbSettings>();

            builder.Services.AddCosmosDb(cosmosDbConfig.Endpoint,
                                          cosmosDbConfig.Key,
                                          cosmosDbConfig.DatabaseName,
                                          cosmosDbConfig.Containers);

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IErpMessageRepository, ErpMessageRepository>();
            builder.Services.AddScoped<ITimeLineRepository, TimeLineRepository>();
            builder.Services.AddScoped<IShopRepository, ShopRepository>();

            this.ConfigureAutoMapper(builder.Services);
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
