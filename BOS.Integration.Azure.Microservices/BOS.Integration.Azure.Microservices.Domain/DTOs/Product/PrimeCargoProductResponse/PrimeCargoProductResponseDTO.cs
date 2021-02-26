namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductResponseDTO
    {
        public string Sku { get; set; }

        public string EnaNo { get; set; }

        public int? ProductId { get; set; }

        public string ErpjobId { get; set; }

        public string ResponseDateTime { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseDescription { get; set; }

        public bool Success { get; set; }
    }
}
