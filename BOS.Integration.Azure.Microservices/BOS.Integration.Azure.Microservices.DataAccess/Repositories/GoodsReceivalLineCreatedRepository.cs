using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class GoodsReceivalLineCreatedRepository : CosmosDbRepository<GoodsReceivalLineCreated>, IGoodsReceivalLineCreatedRepository
    {
        public override string ContainerName { get; } = "webhook";

        public override string GenerateId(GoodsReceivalLineCreated entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public GoodsReceivalLineCreatedRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
