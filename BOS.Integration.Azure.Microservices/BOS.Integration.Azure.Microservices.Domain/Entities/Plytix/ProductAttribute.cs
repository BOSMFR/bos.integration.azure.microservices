using Newtonsoft.Json;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Plytix
{
    public class ProductAttribute : BaseEntity
    {
        [JsonProperty(PropertyName = "filter_type")]
        public string FilterType { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type_class")]
        public string TypeClass { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public List<string> Groups { get; set; }

        [JsonProperty(PropertyName = "options")]
        public List<string> Options { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public List<Attribute> Attributes { get; set; }
    }
}
