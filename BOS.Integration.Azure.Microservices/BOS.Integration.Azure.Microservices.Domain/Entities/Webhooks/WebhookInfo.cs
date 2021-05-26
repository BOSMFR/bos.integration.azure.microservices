using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks
{
    public class WebhookInfo : BaseEntity
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public string RequestBody { get; set; }

        public DateTime ReceivedAt { get; set; }
    }
}
