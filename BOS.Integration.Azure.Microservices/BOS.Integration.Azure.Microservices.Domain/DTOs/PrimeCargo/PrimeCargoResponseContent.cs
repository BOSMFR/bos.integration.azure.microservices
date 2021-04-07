using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo
{
    public class PrimeCargoResponseContent<T> : HttpResponse
    {
        public T Data { get; set; }

        public List<PrimeCargoResponseDetails> ProcessingDetails { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
