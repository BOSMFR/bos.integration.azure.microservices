using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Plytix
{
    public class AssetCategory : BaseEntity
    {
        [JsonProperty(PropertyName = "plytixInstanceId")]
        public string PlytixInstanceId { get; set; }

        [JsonProperty(PropertyName = "n_children")]
        public int ChildrenNumber { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
