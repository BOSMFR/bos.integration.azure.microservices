using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<TimeLine>> GetByFilterAsync(TimeLineRequestDTO timeLineRequest)
        {
            var iterator = _container.GetItemLinqQueryable<TimeLine>()
                                            .Where(t => (timeLineRequest.Objects.Count == 0 || timeLineRequest.Objects.Contains(t.Object.ToLower()))
                                                        && (timeLineRequest.Statuses.Count == 0 || timeLineRequest.Statuses.Contains(t.Status.ToLower())))
                                            .ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
