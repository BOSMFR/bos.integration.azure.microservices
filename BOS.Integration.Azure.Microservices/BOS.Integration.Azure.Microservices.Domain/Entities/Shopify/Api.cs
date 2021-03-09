using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Shopify
{
    public class Api
    {
        [JsonProperty(PropertyName = "serverUrl")]
        public string ServerUrl { get; set; }
    }
}
