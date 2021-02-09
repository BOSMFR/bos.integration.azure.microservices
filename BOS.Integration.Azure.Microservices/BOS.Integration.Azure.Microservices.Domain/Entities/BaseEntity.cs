using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public abstract class BaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }
    }
}
