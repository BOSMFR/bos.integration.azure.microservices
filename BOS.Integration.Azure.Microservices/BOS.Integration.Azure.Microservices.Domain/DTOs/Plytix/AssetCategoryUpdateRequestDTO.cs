using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class AssetCategoryUpdateRequestDTO
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
