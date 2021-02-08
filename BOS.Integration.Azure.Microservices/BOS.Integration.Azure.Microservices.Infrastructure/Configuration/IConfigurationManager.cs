namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public interface IConfigurationManager
    {
        string ServiceBusConnectionString { get; }

        string ServiceBusQueueName { get; }

        CosmosDbSettings CosmosDbSettings { get; }
    }
}
