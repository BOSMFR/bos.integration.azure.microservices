using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class GoodsReceivalClosedDTO
    {
        public int GoodsReceivalId { get; set; }

        public string ReceivalNumber { get; set; }

        public int ReceivalTypeId { get; set; }

        public DateTime FinishedTime { get; set; }
    }
}
