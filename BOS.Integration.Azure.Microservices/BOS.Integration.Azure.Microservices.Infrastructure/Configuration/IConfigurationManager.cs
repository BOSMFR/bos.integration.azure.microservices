namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public interface IConfigurationManager
    {
        string ServiceBusConnectionString { get; }

        string AzureStorageConnectionString { get; }

        string AzureMainBlobContainer { get; }

        CosmosDbSettings CosmosDbSettings { get; }

        PrimeCargoSettings PrimeCargoSettings { get; }

        PlytixSettings PlytixSettings { get; }

        NavSettings NavSettings { get; }
    }
}
