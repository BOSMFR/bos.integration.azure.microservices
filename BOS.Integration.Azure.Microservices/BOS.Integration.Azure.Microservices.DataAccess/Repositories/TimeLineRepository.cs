using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;
using System;

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
    }
}
