using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPickOrderService
    {
        Task<ActionExecutionResult> CreatePickOrderAsync(PickOrderDTO pickOrderDTO);

        Task<ActionExecutionResult> CreatePickOrderFromPrimeCargoInfoAsync(PrimeCargoPickOrderResponseDTO primeCargoResponseObject);

        Task<bool> UpdatePickOrderFromPrimeCargoInfoAsync(PrimeCargoPickOrderResponseDTO primeCargoResponseObject, PickOrder pickOrder = null);

        Task<List<PickOrder>> GetPickOrdersByFilterAsync(PickOrderFilterDTO pickOrderFilter);

        Task<PickOrder> GetPickOrderByIdAsync(string id);

        Task<bool> SetPickOrderClosedAsync(PickOrder pickOrder);
    }
}
