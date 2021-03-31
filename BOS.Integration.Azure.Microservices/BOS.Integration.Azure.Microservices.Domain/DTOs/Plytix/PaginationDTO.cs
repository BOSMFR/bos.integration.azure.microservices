using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class PaginationDTO
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("order")]
        public string Order { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("page_size")]
        public int PageSize { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
