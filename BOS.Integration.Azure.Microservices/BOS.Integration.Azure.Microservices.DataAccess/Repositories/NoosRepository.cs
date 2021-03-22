using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Noos;
using Microsoft.Azure.Cosmos;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class NoosRepository : CosmosDbRepository<Noos>, INoosRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(Noos entity) => entity.StyleNo;

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public NoosRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
