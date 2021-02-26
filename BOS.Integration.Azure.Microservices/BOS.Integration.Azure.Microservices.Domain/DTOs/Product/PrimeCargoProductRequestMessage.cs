namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductRequestMessage
    {
        public LogInfo ErpInfo { get; set; }

        public PrimeCargoProductRequestDTO PrimeCargoProduct { get; set; }
    }
}
