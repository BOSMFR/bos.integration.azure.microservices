using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using Microsoft.Azure.Cosmos;

namespace BOS.Integration.Azure.Microservices.DataAccess
{
    public class CosmosDbContainer : ICosmosDbContainer
    {
        public Container Container { get; }

        public CosmosDbContainer(CosmosClient cosmosClient,
                                 string databaseName,
                                 string containerName)
        {
            this.Container = cosmosClient.GetContainer(databaseName, containerName);
        }
    }
}
