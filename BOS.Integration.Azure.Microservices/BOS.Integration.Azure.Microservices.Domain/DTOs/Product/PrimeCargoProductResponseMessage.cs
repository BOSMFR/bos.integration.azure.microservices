namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductResponseMessage
    {
        public LogInfo ErpInfo { get; set; }

        public PrimeCargoProductResponseDTO PrimeCargoProduct { get; set; }
    }
}
