using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DocumentClient documentClient;
        private readonly string endpoint = "https://bos-integration-cosmos-db-dev.documents.azure.com:443/";
        private readonly string key = "Zfo4WBwTYZ83d3WrcT7Ud4f4bkSa1wX6ZyabgnFDKoCP4PWgEnopEMpFZPJbOnlPXNiaJswXtEXENZp0VaBOYQ==";

        private readonly string databaseName = "bos-integration-sku-cosmos-db-dev";
        private readonly string collectionName = "customer";

        public CustomerRepository()
        {
            documentClient = new DocumentClient(new Uri(endpoint), key);
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(Customer customer)
        {
            await documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                customer
            );
        }

        public Task UpdateAsync(int id, Customer item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
