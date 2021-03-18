using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod
{
    public class DeliveryPeriodDTO
    {
        public string ErpRecordEvent { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public ICollection<DeliveryPeriodDetailsDTO> DeliveryWindow { get; set; }
    }
}
