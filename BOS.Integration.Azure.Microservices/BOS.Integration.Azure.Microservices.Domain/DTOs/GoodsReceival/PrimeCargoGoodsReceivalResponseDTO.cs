using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class PrimeCargoGoodsReceivalResponseDTO
    {
        public int GoodsReceivalId { get; set; }

        public string ReceivalNumber { get; set; }

        public int ReceivalTypeId { get; set; }

        public string Eta { get; set; }

        public bool Finished { get; set; }

        public string ReturnPickOrderNumber { get; set; }

        public string CreateTime { get; set; }

        public string GoodsReceiptStarted { get; set; }

        public string FinishedTime { get; set; }

        public string CreateTimeUtc { get; set; }

        public string GoodsReceiptStartedUTC { get; set; }

        public string FinishedTimeUtc { get; set; }

        public List<PrimeCargoPurchaseLineResponseDTO> Lines { get; set; }
    }
}
