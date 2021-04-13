using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks
{
    public class GoodsReceivalClosed : BaseEntity
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        public int GoodsReceivalId { get; set; }

        public string ReceivalNumber { get; set; }

        public int ReceivalTypeId { get; set; }

        public DateTime FinishedTime { get; set; }
    }
}
