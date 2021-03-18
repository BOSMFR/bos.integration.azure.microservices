using Newtonsoft.Json;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class ProductAttributeUpdateRequestDTO
    {
        [JsonProperty(PropertyName = "options")]
        public IEnumerable<string> Options { get; set; }
    }
}
