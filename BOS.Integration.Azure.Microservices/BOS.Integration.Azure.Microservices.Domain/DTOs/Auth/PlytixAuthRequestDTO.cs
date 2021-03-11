using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Auth
{
    public class PlytixAuthRequestDTO
    {
        [JsonProperty(PropertyName = "api_key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "api_password")]
        public string Password { get; set; }
    }
}
