using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class ProductFilterDTO
    {
        public string Sku { get; set; }

        public string EanNo { get; set; }

        public int? ProductId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
