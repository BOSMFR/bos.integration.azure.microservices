namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public interface IConfigurationManager
    {
        string ServiceBusConnectionString { get; }

        ServiceBusSettings ServiceBusSettings { get; }

        CosmosDbSettings CosmosDbSettings { get; }
    }
}
