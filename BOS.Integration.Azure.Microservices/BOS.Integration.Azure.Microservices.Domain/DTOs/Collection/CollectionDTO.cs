using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Collection
{
    public class CollectionDTO
    {
        public string ErpRecordEvent { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public ICollection<CollectionDetailsDTO> Season { get; set; }
    }
}
