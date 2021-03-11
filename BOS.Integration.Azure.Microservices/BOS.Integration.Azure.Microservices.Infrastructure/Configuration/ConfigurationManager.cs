using Microsoft.Extensions.Configuration;

namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ServiceBusConnectionString => this.configuration.GetConnectionString("servicebus");

        public string AzureStorageConnectionString => this.configuration.GetConnectionString("storage");

        public string AzureMainBlobContainer => this.configuration["AzureBlobContainer"];

        public CosmosDbSettings CosmosDbSettings => this.GetSectionByName<CosmosDbSettings>("CosmosDbConfig");

        public PrimeCargoSettings PrimeCargoSettings => this.GetSectionByName<PrimeCargoSettings>("PrimeCargoApiConfig");

        public PlytixSettings PlytixSettings => this.GetSectionByName<PlytixSettings>("PlytixApiConfig");

        private T GetSectionByName<T>(string sectionName)
            where T : class
        {
            return this.configuration.GetSection(sectionName).Get<T>();
        }
    }
}
