using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class AssetCategoryRepository : CosmosDbRepository<AssetCategory>, IAssetCategoryRepository
    {
        public override string ContainerName { get; } = "assetCategory";

        public override string GenerateId(AssetCategory entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public AssetCategoryRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
