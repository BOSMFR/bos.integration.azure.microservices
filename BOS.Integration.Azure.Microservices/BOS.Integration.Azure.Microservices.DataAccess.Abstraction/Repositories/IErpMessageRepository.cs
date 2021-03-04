using BOS.Integration.Azure.Microservices.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IErpMessageRepository : IRepository<ErpMessage>
    {
        Task<List<ErpMessage>> GetByObjectIdAsync(string id);
    }
}
