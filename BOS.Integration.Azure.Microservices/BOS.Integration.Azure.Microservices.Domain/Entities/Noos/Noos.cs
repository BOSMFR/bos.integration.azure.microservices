using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Noos
{
    public class Noos : ErpEntity
    {
        public string StyleNo { get; set; }

        public string StyleDescription { get; set; }

        public string ColourTable { get; set; }

        public string ErpRecordEvent { get; set; }

        public List<NoosDetails> Details { get; set; }
    }
}
