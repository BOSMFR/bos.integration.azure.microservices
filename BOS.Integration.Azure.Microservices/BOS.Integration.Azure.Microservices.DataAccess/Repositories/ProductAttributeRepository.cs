using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ProductAttributeRepository : CosmosDbRepository<ProductAttribute>, IProductAttributeRepository
    {
        public override string ContainerName { get; } = "productAttribute";

        public override string GenerateId(ProductAttribute entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public ProductAttributeRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
