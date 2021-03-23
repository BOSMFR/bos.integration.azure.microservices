using Newtonsoft.Json;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class PlytixSearchRequestDTO
    {
        [JsonProperty(PropertyName = "attributes")]
        public ICollection<string> Attributes { get; set; }
    }
}
