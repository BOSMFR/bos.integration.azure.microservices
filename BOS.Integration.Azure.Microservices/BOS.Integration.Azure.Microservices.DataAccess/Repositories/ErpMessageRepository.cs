using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class ErpMessageRepository : CosmosDbRepository<ErpMessage>, IErpMessageRepository
    {
        public override string ContainerName { get; } = "erpMessage";

        public override string GenerateId(ErpMessage entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public ErpMessageRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<ErpMessage>> GetByObjectIdAsync(string id)
        {
            var iterator = _container.GetItemLinqQueryable<ErpMessage>().Where(x => x.ObjectId == id).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
