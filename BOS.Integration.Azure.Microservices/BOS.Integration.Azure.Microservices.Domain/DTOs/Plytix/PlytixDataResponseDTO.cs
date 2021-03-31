using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class PlytixDataResponseDTO<T> : HttpResponse
    {
        public ICollection<T> Data { get; set; }
    }
}
