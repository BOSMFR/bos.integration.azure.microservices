using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
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
    public class TimeLineRepository : CosmosDbRepository<TimeLine>, ITimeLineRepository
    {
        public override string ContainerName { get; } = "timeLine";

        public override string GenerateId(TimeLine entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey = null) => new PartitionKey(partitionKey);

        public TimeLineRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<TimeLine>> GetByFilterAsync(TimeLineFilterDTO timeLineFilter)
        {
            Expression<Func<TimeLine, bool>> query = t => (t.DateTime > timeLineFilter.FromDate && t.DateTime < timeLineFilter.ToDate)
                                                        && (timeLineFilter.Objects.Count == 0 || timeLineFilter.Objects.Contains(t.Object.ToLower()))
                                                        && (timeLineFilter.Statuses.Count == 0 || timeLineFilter.Statuses.Contains(t.Status.ToLower()));

            var iterator = _container.GetItemLinqQueryable<TimeLine>().Where(query).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }

        public async Task<List<TimeLine>> GetByObjectIdAsync(string id)
        {
            var iterator = _container.GetItemLinqQueryable<TimeLine>().Where(x => x.ObjectId == id).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
