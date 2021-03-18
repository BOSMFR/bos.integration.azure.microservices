using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IPlytixRepository : IRepository<Plytix>
    {
        Task<List<PlytixInstance>> GetActiveInstancesAsync();
    }
}
