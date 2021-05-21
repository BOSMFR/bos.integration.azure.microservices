using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class PickOrderRepository : CosmosDbRepository<PickOrder>, IPickOrderRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(PickOrder entity) => entity.OrderNumber;

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public PickOrderRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<PickOrder>> GetByFilterAsync(PickOrderFilterDTO pickOrderFilter, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            Expression<Func<PickOrder, bool>> query = g => (g.ReceivedFromErp >= pickOrderFilter.FromDate && g.ReceivedFromErp < pickOrderFilter.ToDate)
                                                        && (string.IsNullOrEmpty(pickOrderFilter.OrderNumber) || g.OrderNumber == pickOrderFilter.OrderNumber)
                                                        && (string.IsNullOrEmpty(pickOrderFilter.CustomerNumber) || g.CustomerNumber == pickOrderFilter.CustomerNumber)
                                                        && (string.IsNullOrEmpty(pickOrderFilter.CustomerId1) || g.CustomerID1 == pickOrderFilter.CustomerId1)
                                                        && (string.IsNullOrEmpty(pickOrderFilter.CustomerId2) || g.CustomerID2 == pickOrderFilter.CustomerId2)
                                                        && (string.IsNullOrEmpty(pickOrderFilter.CustomerId3) || g.CustomerID3 == pickOrderFilter.CustomerId3)
                                                        && (string.IsNullOrEmpty(pickOrderFilter.ReceiverName) || g.ReceiverName.Contains(pickOrderFilter.ReceiverName));

            var iterator = _container.GetItemLinqQueryable<PickOrder>(requestOptions: requestOptions).Where(query).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
