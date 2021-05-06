using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class GoodsReceivalFilterDTO
    {
        public string WmsDocumentNo { get; set; }

        public int? PrimeCargoGoodsReceivalId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
