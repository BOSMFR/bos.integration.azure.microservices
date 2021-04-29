namespace BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival
{
    public class PurchaseLine
    {
        public string WmsDocumentLineNo { get; set; }

        public int OrderLineNo { get; set; }

        public int ReceiveListLineNo { get; set; }

        public int PrimeCargoProductId { get; set; }

        public bool IsAssortment { get; set; }

        public string ItemNo { get; set; }

        public string Sku { get; set; }

        public string Ean { get; set; }

        public string Description { get; set; }

        public string LocationCode { get; set; }

        public double QtyToReceive { get; set; }

        public int QuantityReceived { get; set; }

        public string AssortmentCode { get; set; }

        public double AssortmentQuantity { get; set; }

        public string AssortmentColour { get; set; }

        public string AssortmentStyle { get; set; }

        public string ColourCode { get; set; }

        public string StyleCode { get; set; }

        public string SizeCode { get; set; }

        public string QualityCode { get; set; }
    }
}
