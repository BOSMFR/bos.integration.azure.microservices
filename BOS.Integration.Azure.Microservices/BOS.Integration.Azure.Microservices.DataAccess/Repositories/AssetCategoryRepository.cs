using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<AssetCategory> GetByNameAsync(string name, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            var iterator = _container.GetItemLinqQueryable<AssetCategory>(requestOptions: requestOptions).Where(x => x.Name.ToLower() == name.ToLower()).ToFeedIterator();

            return (await iterator.ReadNextAsync()).FirstOrDefault();
        }
    }
}
