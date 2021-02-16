using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction
{
    public interface ICosmosDbContainerFactory
    {
        ICosmosDbContainer GetContainer(string containerName);

        Task EnsureDbSetupAsync();
    }
}
