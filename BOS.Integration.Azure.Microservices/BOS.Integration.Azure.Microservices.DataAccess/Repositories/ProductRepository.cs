using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ProductRepository : CosmosDbRepository<Product>, IProductRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(Product entity) => entity.EanNo;

        public override PartitionKey ResolvePartitionKey(string partitionKey = null) => new PartitionKey(NavObjectCategory.Sku);

        public ProductRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<Product>> GetAllByPrimeCargoIntegrationStateAsync(string state)
        {
            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = this.ResolvePartitionKey()
            };

            var iterator = _container.GetItemLinqQueryable<Product>(requestOptions: requestOptions)
                                        .Where(p => p.PrimeCargoIntegration.State == state)
                                        .ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
