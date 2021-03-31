using Newtonsoft.Json;
using System.Collections.Generic;

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

        [JsonProperty(PropertyName = "order")]
        public string Order { get; set; }

        [JsonProperty(PropertyName = "path")]
        public IEnumerable<string> Path { get; set; }

        [JsonProperty(PropertyName = "parents_ids")]
        public IEnumerable<string> ParentsIds { get; set; }
    }
}
