using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class FileDTO
    {
        [JsonProperty(PropertyName = "filename")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
