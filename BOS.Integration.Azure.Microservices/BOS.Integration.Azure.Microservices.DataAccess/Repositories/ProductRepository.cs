using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ProductRepository : CosmosDbRepository<Product>, IProductRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(Product entity) => entity.EanNo;

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public ProductRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<Product>> GetAllByPrimeCargoIntegrationStateAsync(string state, string category = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(category))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(category)
                };
            }

            var iterator = _container.GetItemLinqQueryable<Product>(requestOptions: requestOptions)
                                        .Where(p => p.PrimeCargoIntegration.State == state)
                                        .ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }

        public async Task<List<Product>> GetByFilterAsync(ProductFilterDTO productFilter, string category = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(category))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(category)
                };
            }

            Expression<Func<Product, bool>> query = p => (p.ReceivedFromErp >= productFilter.FromDate && p.ReceivedFromErp < productFilter.ToDate)
                                                        && (!productFilter.ProductId.HasValue || p.PrimeCargoProductId == productFilter.ProductId)
                                                        && (string.IsNullOrEmpty(productFilter.EanNo) || p.EanNo == productFilter.EanNo)
                                                        && (string.IsNullOrEmpty(productFilter.Sku) || p.Sku.Contains(productFilter.Sku));

            var iterator = _container.GetItemLinqQueryable<Product>(requestOptions: requestOptions).Where(query).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
