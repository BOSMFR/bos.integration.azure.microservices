using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Auth
{
    public class PlytixAuthData
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
