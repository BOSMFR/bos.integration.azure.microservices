using Newtonsoft.Json;
using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public abstract class AssetEntity : BaseEntity
    {
        [JsonProperty(PropertyName = "assetType")]
        public string AssetType { get; set; }

        public string Created { get; set; }

        public DateTime ReceivedFromSsis { get; set; }
    }
}
