using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductResponseContent
    {
        public PrimeCargoProductResponseData Data { get; set; }

        public List<PrimeCargoProductResponseDetail> ProcessingDetails { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }
    }
}
