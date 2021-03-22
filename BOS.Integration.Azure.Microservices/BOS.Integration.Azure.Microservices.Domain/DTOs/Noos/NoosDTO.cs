using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Noos
{
    public class NoosDTO
    {
        public string ItemNo { get; set; }

        public string ItemDescription { get; set; }

        public string ColourTable { get; set; }

        public string ErpRecordEvent { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public List<NoosDetailsDTO> Details { get; set; }
    }
}
