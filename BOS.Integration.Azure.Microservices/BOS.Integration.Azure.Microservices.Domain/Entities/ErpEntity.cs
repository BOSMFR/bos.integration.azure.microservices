using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public abstract class ErpEntity : BaseEntity
    {
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public DateTime ReceivedFromErp { get; set; }
    }
}
