using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class PrimeCargoGoodsReceivalRequestDTO
    {
        public string ReceivalNumber { get; set; }

        public int ReceivalTypeId { get; set; }

        public string Eta { get; set; }

        public List<PrimeCargoPurchaseLineRequestDTO> Lines { get; set; }
    }
}
