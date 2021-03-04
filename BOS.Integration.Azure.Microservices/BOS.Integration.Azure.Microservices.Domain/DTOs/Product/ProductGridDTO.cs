using BOS.Integration.Azure.Microservices.Domain.Entities.Product;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class ProductGridDTO
    {
        public string Id { get; set; }

        public string Sku { get; set; }

        public string EanNo { get; set; }

        public int? ProductId { get; set; }

        public Characteristic Style { get; set; }
    }
}
