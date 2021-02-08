using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfigurationManager configurationManager;
        private readonly DocumentClient documentClient;

        private readonly string collectionName = "product";

        public ProductRepository(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            documentClient = new DocumentClient(new Uri(configurationManager.CosmosDbSettings.Endpoint), configurationManager.CosmosDbSettings.Key);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(Product product)
        {
            await documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(configurationManager.CosmosDbSettings.DatabaseName, collectionName),
                product
            );
        }

        public Task UpdateAsync(int id, Product item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
