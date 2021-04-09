using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Plytix
{
    public class PlytixInstance
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("serverUrl")]
        public string ServerUrl { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("paswword")]
        public string Paswword { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
