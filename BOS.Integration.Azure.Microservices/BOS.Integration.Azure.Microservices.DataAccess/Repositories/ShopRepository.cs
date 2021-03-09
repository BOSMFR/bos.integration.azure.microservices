using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Shopify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ShopRepository : CosmosDbRepository<Shop>, IShopRepository
    {
        public override string ContainerName { get; } = "B2cShopifyShop";

        public override string GenerateId(Shop entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey = null) => new PartitionKey(partitionKey);

        public ShopRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<Shop>> GetAllActiveAsync(string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            var iterator = _container.GetItemLinqQueryable<Shop>(requestOptions: requestOptions).Where(x => x.Active).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
