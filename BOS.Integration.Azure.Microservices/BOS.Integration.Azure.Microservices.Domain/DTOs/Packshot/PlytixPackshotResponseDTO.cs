using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PlytixPackshotResponseDTO : HttpResponse
    {
        public ICollection<PlytixPackshotResponseData> Data { get; set; }
    }
}
