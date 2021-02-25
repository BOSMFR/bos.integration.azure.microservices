using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ErpMessageRepository : CosmosDbRepository<ErpMessage>, IErpMessageRepository
    {
        public override string ContainerName { get; } = "erpMessage";

        public override string GenerateId(ErpMessage entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey = null) => new PartitionKey(partitionKey);

        public ErpMessageRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
