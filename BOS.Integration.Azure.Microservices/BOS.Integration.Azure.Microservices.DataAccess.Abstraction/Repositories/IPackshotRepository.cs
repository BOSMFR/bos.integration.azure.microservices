using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IPackshotRepository : IRepository<Packshot>
    {
        Task<Packshot> GetByKeyParamsAsync(Packshot packshot, string partitionKey = null);
    }
}
