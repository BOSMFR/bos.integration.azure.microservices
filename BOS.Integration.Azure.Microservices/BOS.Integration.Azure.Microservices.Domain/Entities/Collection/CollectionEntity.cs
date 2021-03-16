using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Collection
{
    public class CollectionEntity : ErpEntity
    {
        public string ErpRecordEvent { get; set; }

        public ICollection<CollectionDetails> Details { get; set; }
    }
}
