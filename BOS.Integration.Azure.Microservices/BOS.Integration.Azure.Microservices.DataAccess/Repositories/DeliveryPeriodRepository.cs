using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class DeliveryPeriodRepository : CosmosDbRepository<DeliveryPeriod>, IDeliveryPeriodRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(DeliveryPeriod entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public DeliveryPeriodRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
