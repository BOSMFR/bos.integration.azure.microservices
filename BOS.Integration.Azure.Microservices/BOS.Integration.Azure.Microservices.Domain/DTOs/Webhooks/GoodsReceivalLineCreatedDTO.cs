using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks
{
    public class GoodsReceivalLineCreatedDTO
    {
        public int GoodsReceivalLineReceivedId { get; set; }

        public int GoodsReceivalId { get; set; }

        public string ReceivalNumber { get; set; }

        public int ReceivalTypeId { get; set; }

        public int GoodsReceivalLineId { get; set; }

        public int ProductId { get; set; }

        public int ExtReference { get; set; }

        public int ReceivedQty { get; set; }

        public string CustomsReference { get; set; }

        public string Batch { get; set; }

        public string OriginalBatch { get; set; }

        public DateTime BestBefore { get; set; }

        public DateTime OriginalBestBefore { get; set; }

        public DateTime PutAwayTime { get; set; }
    }
}
