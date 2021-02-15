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

        public CosmosDbSettings CosmosDbSettings => this.GetSectionByName<CosmosDbSettings>("CosmosDbConfig");

        public ServiceBusSettings ServiceBusSettings => this.GetSectionByName<ServiceBusSettings>("ServiceBusConfig");

        private T GetSectionByName<T>(string sectionName)
            where T : class
        {
            return this.configuration.GetSection(sectionName).Get<T>();
        }
    }
}
