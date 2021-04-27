using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class PlytixRepository : CosmosDbRepository<Plytix>, IPlytixRepository
    {
        public override string ContainerName { get; } = "plytix";

        public override string GenerateId(Plytix entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public PlytixRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<PlytixInstance> GetActiveInstanceAsync()
        {
            var iterator = _container.GetItemLinqQueryable<Plytix>().ToFeedIterator();

            var plytix = (await iterator.ReadNextAsync()).FirstOrDefault();

            return plytix?.PlytixInstances?.Where(x => x.Active)?.FirstOrDefault();
        }
    }
}
