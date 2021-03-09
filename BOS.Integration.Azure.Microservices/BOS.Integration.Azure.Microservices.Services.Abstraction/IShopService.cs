using BOS.Integration.Azure.Microservices.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IShopService
    {
        Task<List<ShopDTO>> GetAllActiveAsync(string partitionKey = null);
    }
}
