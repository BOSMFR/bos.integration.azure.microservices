using Newtonsoft.Json;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class PlytixSearchRequestDTO
    {
        [JsonProperty(PropertyName = "attributes")]
        public IReadOnlyCollection<string> Attributes { get; set; }

        [JsonProperty(PropertyName = "pagination")]
        public PaginationDTO Pagination { get; set; }
    }
}
