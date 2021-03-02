using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public abstract class CosmosDbRepository<T> : IRepository<T>, IContainerContext<T> where T : BaseEntity
    {
        public abstract string ContainerName { get; }

        public abstract string GenerateId(T entity);

        public abstract PartitionKey ResolvePartitionKey(string partitionKey = null);

        protected readonly Container _container;

        public CosmosDbRepository(ICosmosDbContainerFactory cosmosDbContainerFactory)
        {
            this._container = cosmosDbContainerFactory.GetContainer(ContainerName).Container;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string partitionKey = null)
        {
            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = this.ResolvePartitionKey(partitionKey)
            };

            FeedIterator<T> resultSetIterator = _container.GetItemQueryIterator<T>(requestOptions: requestOptions);
            List<T> results = new List<T>();

            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<T> response = await resultSetIterator.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<T> GetByIdAsync(string id, string partitionKey = null)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, ResolvePartitionKey(partitionKey));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddAsync(T item, string partitionKey = null)
        {
            item.Id = GenerateId(item);
            await _container.CreateItemAsync<T>(item, ResolvePartitionKey(partitionKey));
        }

        public async Task AddRangeAsync(ICollection<T> items, string partitionKey = null)
        {
            var tasks = new List<Task>();

            foreach (var itemToInsert in items)
            {
                itemToInsert.Id = GenerateId(itemToInsert);
                tasks.Add(_container.CreateItemAsync(itemToInsert, ResolvePartitionKey(partitionKey)));
            }

            await Task.WhenAll(tasks);
        }

        public async Task UpdateAsync(string id, T item, string partitionKey = null)
        {
            await this._container.UpsertItemAsync<T>(item, ResolvePartitionKey(partitionKey));
        }

        public async Task DeleteAsync(string id, string partitionKey = null)
        {
            await this._container.DeleteItemAsync<T>(id, ResolvePartitionKey(partitionKey));
        }
    }
}
