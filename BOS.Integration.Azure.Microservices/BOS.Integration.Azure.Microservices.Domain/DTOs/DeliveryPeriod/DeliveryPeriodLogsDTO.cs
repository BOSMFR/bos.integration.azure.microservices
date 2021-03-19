using DeliveryPeriodEntity = BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod.DeliveryPeriod;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod
{
    public class DeliveryPeriodLogsDTO
    {
        public DeliveryPeriodEntity DeliveryPeriod { get; set; }

        public LogDTO Logs { get; set; }
    }
}
