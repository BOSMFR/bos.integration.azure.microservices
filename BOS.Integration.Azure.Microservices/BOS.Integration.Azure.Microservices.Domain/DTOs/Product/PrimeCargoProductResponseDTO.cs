namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductResponseDTO
    {
        public string Sku { get; set; }

        public string EnaNo { get; set; }

        public int? ProductId { get; set; }

        public string ErpjobId { get; set; }

        public string ResponceDateTime { get; set; }

        public string ResponceCode { get; set; }

        public string ResponceDescription { get; set; }

        public bool Success { get; set; }
    }
}
