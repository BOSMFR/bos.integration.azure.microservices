using BOS.Integration.Azure.Microservices.DataAccess;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Functions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Register a singleton instance of Cosmos Db Container Factory, which is a wrapper for the CosmosClient.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="endpointUrl"></param>
        /// <param name="primaryKey"></param>
        /// <param name="databaseName"></param>
        /// <param name="containers"></param>
        /// <returns></returns>
        public static IServiceCollection AddCosmosDb(this IServiceCollection services,
                                                     string endpointUrl,
                                                     string primaryKey,
                                                     string databaseName,
                                                     List<ContainerInfo> containers)
        {
            CosmosClient client = new CosmosClient(endpointUrl, primaryKey);
            CosmosDbContainerFactory cosmosDbClientFactory = new CosmosDbContainerFactory(client, databaseName, containers);

            cosmosDbClientFactory.EnsureDbSetupAsync().Wait();

            services.AddSingleton<ICosmosDbContainerFactory>(cosmosDbClientFactory);

            return services;
        }
    }
}
