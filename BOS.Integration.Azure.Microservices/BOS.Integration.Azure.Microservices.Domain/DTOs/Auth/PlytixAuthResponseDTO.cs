using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Auth
{
    public class PlytixAuthResponseDTO : HttpResponse
    {
        public ICollection<PlytixAuthData> Data { get; set; }
    }
}
