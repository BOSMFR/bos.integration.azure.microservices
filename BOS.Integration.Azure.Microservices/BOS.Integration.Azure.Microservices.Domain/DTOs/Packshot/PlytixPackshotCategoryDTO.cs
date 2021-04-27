using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PlytixPackshotCategoryDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
