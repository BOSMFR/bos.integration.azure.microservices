using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ProductRepository : CosmosDbRepository<Product>, IProductRepository
    {
        public override string ContainerName { get; } = "product";

        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);

        public ProductRepository(ICosmosDbContainerFactory factory) 
            : base(factory)
        {
        }
    }
}
