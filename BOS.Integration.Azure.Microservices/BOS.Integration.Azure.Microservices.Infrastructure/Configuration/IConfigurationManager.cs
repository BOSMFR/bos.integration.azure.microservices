namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public interface IConfigurationManager
    {
        string ServiceBusConnectionString { get; }

        CosmosDbSettings CosmosDbSettings { get; }
    }
}
