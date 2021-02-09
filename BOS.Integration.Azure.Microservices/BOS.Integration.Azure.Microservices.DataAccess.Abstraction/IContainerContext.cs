using Microsoft.Azure.Cosmos;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction
{
    public interface IContainerContext
    {
        string ContainerName { get; }

        PartitionKey ResolvePartitionKey(string entityId);
    }
}
