using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction
{
    public interface IContainerContext<T> where T : BaseEntity
    {
        string ContainerName { get; }

        string GenerateId(T entity);

        PartitionKey ResolvePartitionKey(string partitionKey = null);
    }
}
