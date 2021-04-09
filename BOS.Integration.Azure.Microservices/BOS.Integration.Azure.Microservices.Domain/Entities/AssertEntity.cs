using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public abstract class AssertEntity : BaseEntity
    {
        [JsonProperty(PropertyName = "assertType")]
        public string AssertType { get; set; }

        public string Created { get; set; }

        public DateTime ReceivedFromSsis { get; set; }
    }
}
