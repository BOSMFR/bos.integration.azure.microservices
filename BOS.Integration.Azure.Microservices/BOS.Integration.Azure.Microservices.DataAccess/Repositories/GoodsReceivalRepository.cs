using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class GoodsReceivalRepository : CosmosDbRepository<GoodsReceival>, IGoodsReceivalRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(GoodsReceival entity) => entity.WmsDocumentNo;

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public GoodsReceivalRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
