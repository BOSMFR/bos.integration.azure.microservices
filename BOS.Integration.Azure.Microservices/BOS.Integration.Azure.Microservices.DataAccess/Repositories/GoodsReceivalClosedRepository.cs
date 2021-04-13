using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class GoodsReceivalClosedRepository : CosmosDbRepository<GoodsReceivalClosed>, IGoodsReceivalClosedRepository
    {
        public override string ContainerName { get; } = "webhook";

        public override string GenerateId(GoodsReceivalClosed entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public GoodsReceivalClosedRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
