using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPrimeCargoService
    {
        Task<PrimeCargoProductResponseDTO> CreateOrUpdatePrimeCargoProductAsync(PrimeCargoProductRequestDTO primeCargoProduct, ActionType actionType);
    }
}
