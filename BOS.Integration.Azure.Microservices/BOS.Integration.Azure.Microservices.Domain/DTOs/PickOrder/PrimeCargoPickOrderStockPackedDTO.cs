using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoPickOrderStockPackedDTO
    {
        public int StockPackedId { get; set; }

        public int CartonId { get; set; }

        public int ProductId { get; set; }

        public int Qty { get; set; }

        public int PickOrderLineId { get; set; }

        public DateTime LastChanged { get; set; }

        public string CustomsReference { get; set; }

        public string Batch { get; set; }

        public DateTime? BestBefore { get; set; }
    }
}
