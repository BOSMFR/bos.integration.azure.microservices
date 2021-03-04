using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface ITimeLineRepository : IRepository<TimeLine>
    {
        Task<List<TimeLine>> GetByObjectIdAsync(string id);

        Task<List<TimeLine>> GetByFilterAsync(TimeLineFilterDTO timeLineFilter);
    }
}
