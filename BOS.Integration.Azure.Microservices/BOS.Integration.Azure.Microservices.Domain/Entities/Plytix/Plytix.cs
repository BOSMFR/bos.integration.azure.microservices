using Newtonsoft.Json;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Plytix
{
    public class Plytix : BaseEntity
    {
        [JsonProperty(PropertyName = "plytixInstances")]
        public List<PlytixInstance> PlytixInstances { get; set; }
    }
}
