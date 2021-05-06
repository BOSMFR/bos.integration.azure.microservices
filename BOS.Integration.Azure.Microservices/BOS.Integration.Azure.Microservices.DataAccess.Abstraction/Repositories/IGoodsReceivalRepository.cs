using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IGoodsReceivalRepository : IRepository<GoodsReceival>
    {
        Task<List<GoodsReceival>> GetByFilterAsync(GoodsReceivalFilterDTO goodsReceivalFilter, string partitionKey = null);
    }
}
