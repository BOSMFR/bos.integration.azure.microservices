using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class CollectionRepository : CosmosDbRepository<CollectionEntity>, ICollectionRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(CollectionEntity entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public CollectionRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
