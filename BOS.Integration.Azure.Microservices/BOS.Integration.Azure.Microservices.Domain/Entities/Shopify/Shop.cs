using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Shopify
{
    public class Shop : BaseEntity
    {
        [JsonProperty(PropertyName = "brand")]
        public string Brand { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }

        [JsonProperty(PropertyName = "api")]
        public Api Api { get; set; }
    }
}
