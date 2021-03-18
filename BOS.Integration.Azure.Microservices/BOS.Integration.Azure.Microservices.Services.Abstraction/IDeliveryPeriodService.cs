using BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IDeliveryPeriodService
    {
        Task<DeliveryPeriod> GetDeliveryPeriodAsync();

        Task<DeliveryPeriod> CreateOrUpdateDeliveryPeriodAsync(DeliveryPeriodDTO deliveryPeriodDTO);
    }
}
