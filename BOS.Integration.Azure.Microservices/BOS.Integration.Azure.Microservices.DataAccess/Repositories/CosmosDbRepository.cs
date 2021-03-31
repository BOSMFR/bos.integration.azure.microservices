using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public abstract class CosmosDbRepository<T> : IRepository<T>, IContainerContext<T> where T : BaseEntity
    {
        public abstract string ContainerName { get; }

        public abstract string GenerateId(T entity);

        public abstract PartitionKey ResolvePartitionKey(string partitionKey);

        protected readonly Container _container;

        public CosmosDbRepository(ICosmosDbContainerFactory cosmosDbContainerFactory)
        {
            this._container = cosmosDbContainerFactory.GetContainer(ContainerName).Container;
        }

        public async Task<ICollection<T>> GetAllAsync(string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            FeedIterator<T> resultSetIterator = _container.GetItemQueryIterator<T>(requestOptions: requestOptions);
            List<T> results = new List<T>();

            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<T> response = await resultSetIterator.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<ICollection<V>> GetAllPropertyValuesAsync<V>(Expression<Func<T, V>> selectExpression, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            var iterator = _container.GetItemLinqQueryable<T>(requestOptions: requestOptions).Select(selectExpression).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }

        public async Task<ICollection<T>> GetAllExceptAsync(ICollection<T> items, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            var iterator = _container.GetItemLinqQueryable<T>(requestOptions: requestOptions).Except(items).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }

        public async Task<T> GetByIdAsync(string id, string partitionKey = null)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, ResolvePartitionKey(partitionKey ?? id));

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
            await _container.CreateItemAsync(item, ResolvePartitionKey(partitionKey ?? item.Id));
        }

        public async Task AddRangeAsync(ICollection<T> items, string partitionKey = null)
        {
            var tasks = new List<Task>();

            foreach (var itemToInsert in items)
            {
                if (string.IsNullOrEmpty(itemToInsert.Id))
                {
                    itemToInsert.Id = GenerateId(itemToInsert);
                }

                tasks.Add(_container.CreateItemAsync(itemToInsert, ResolvePartitionKey(partitionKey ?? itemToInsert.Id)));
            }

            await Task.WhenAll(tasks);
        }

        public async Task UpdateAsync(T item, string partitionKey = null)
        {
            await this._container.UpsertItemAsync(item, ResolvePartitionKey(partitionKey ?? item.Id));
        }

        public async Task UpdateRangeAsync(ICollection<T> items, string partitionKey = null)
        {
            var tasks = new List<Task>();

            foreach (var itemToInsert in items)
            {
                if (string.IsNullOrEmpty(itemToInsert.Id))
                {
                    itemToInsert.Id = GenerateId(itemToInsert);
                }
                
                tasks.Add(_container.UpsertItemAsync(itemToInsert, ResolvePartitionKey(partitionKey ?? itemToInsert.Id)));
            }

            await Task.WhenAll(tasks);
        }

        public async Task DeleteAsync(string id, string partitionKey = null)
        {
            await this._container.DeleteItemAsync<T>(id, ResolvePartitionKey(partitionKey ?? id));
        }

        public async Task DeleteRangeAsync(ICollection<string> ids, string partitionKey = null)
        {
            var tasks = new List<Task>();

            foreach (var id in ids)
            {
                tasks.Add(_container.DeleteItemAsync<T>(id, ResolvePartitionKey(partitionKey ?? id)));
            }

            await Task.WhenAll(tasks);
        }
    }
}
