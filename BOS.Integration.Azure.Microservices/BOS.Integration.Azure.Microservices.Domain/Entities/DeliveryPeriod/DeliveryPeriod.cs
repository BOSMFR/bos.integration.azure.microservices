using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod
{
    public class DeliveryPeriod : ErpEntity
    {
        public string ErpRecordEvent { get; set; }

        public ICollection<DeliveryPeriodDetails> Details { get; set; }
    }
}
