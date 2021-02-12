using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ProductRepository : CosmosDbRepository<Product>, IProductRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(Product entity) => $"{entity.Category}:{Guid.NewGuid()}";

        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':').First());

        public ProductRepository(ICosmosDbContainerFactory factory) 
            : base(factory)
        {
        }

        public async Task<Product> GetByEanNoAsync(string eanNo)
        {
            var iterator = _container.GetItemLinqQueryable<Product>()
                                        .Where(p => p.EanNo == eanNo)
                                        .ToFeedIterator();

            return (await iterator.ReadNextAsync()).FirstOrDefault();
        }
    }
}
