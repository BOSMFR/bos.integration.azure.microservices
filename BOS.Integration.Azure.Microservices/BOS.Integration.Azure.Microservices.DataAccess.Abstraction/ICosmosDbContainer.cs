using Microsoft.Azure.Cosmos;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction
{
    public interface ICosmosDbContainer
    {
        Container Container { get; }
    }
}
