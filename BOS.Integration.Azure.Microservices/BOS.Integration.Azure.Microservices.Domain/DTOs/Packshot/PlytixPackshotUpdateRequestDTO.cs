using Newtonsoft.Json;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PlytixPackshotUpdateRequestDTO
    {
        [JsonProperty(PropertyName = "categories")]
        public List<PlytixPackshotCategoryDTO> Categories { get; set; }
    }
}
