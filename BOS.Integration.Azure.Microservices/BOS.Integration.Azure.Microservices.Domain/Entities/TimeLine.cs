using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public class TimeLine : BaseEntity
    {
        [JsonProperty(PropertyName = "object")]
        public string Object { get; set; }

        public string ObjectId { get; set; }

        public string ErpjobId { get; set; }

        public DateTime DateTime { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }
}
