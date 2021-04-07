namespace BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival
{
    public class PurchaseVariant
    {
        public string ColourTable { get; set; }

        public string SizeTable { get; set; }

        public string StyleTable { get; set; }

        public string ColourCode { get; set; }

        public string AssortmentColourDescription { get; set; }

        public string SizeCode { get; set; }

        public string StyleCode { get; set; }

        public string QualityCode { get; set; }

        public string AssortmentCode { get; set; }

        public string EanNo { get; set; }

        public string Sku { get; set; }

        public string PrimeCargoProductId { get; set; }

        public string StartDatePrimeCargoExport { get; set; }
    }
}
