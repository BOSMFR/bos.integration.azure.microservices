using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface INavService
    {
        Task<ActionExecutionResult> UpdateSkuIntoNavAsync(string eanNo, string productId);

        Task<ActionExecutionResult> UpdateGoodsReceivalIntoNavAsync(PrimeCargoGoodsReceivalResponseDTO primeCargoResponse);

        Task<ActionExecutionResult> UpdatePickOrderIntoNavAsync(PrimeCargoPickOrderResponseDTO primeCargoResponse);
    }
}
