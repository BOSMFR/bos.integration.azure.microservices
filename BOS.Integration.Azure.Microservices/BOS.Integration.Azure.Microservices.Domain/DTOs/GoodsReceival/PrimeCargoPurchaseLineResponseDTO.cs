namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class PrimeCargoPurchaseLineResponseDTO
    {
        public int GoodsReceivalLineId { get; set; }

        public int GoodsReceivalId { get; set; }

        public int ExtReference { get; set; }

        public int Qty { get; set; }

        public int ProductId { get; set; }

        public bool Finished { get; set; }

        public string ReasonCode { get; set; }

        public string CustomsReference { get; set; }

        public int ProductBarcodeId { get; set; }

        public string BatchExpected { get; set; }

        public string BestBeforeExpected { get; set; }

        public bool AddedManually { get; set; }

        public int AmountReceived { get; set; }

        public string LastReceived { get; set; }
    }
}
