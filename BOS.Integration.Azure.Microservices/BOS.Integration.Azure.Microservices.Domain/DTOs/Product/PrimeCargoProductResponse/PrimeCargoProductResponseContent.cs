namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductResponseContent
    {
        public PrimeCargoProductResponseData Data { get; set; }

        public PrimeCargoProductResponseDetails ProcessingDetails { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }
    }
}
