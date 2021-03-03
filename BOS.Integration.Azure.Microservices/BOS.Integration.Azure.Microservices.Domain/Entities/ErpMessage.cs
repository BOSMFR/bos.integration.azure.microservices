using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public class ErpMessage : BaseEntity
    {
        [JsonProperty(PropertyName = "object")]
        public string Object { get; set; }

        public string ObjectId { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public DateTime ReceivedFromErp { get; set; }

        public string Status { get; set; }
    }
}
