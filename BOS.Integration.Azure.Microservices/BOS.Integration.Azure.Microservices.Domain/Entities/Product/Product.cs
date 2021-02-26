using Newtonsoft.Json;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Product
{
    public class Product : BaseEntity
    {
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        public string Sku { get; set; }

        public string EanNo { get; set; }

        public string NoSeries { get; set; }

        public string ItemNo { get; set; }

        public string Description { get; set; }

        public int? PrimeCargoProductId { get; set; }

        public string WmsProductType { get; set; }

        public string StartDatePrimeCargoExport { get; set; }

        public string StartDatePmiExport { get; set; }

        public bool Fifo { get; set; }

        public bool GiveAway { get; set; }

        public bool ExtraService { get; set; }

        public int? AzureOutboundStatusId { get; set; }

        public string AzureOutboundStatusText { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public string ReceivedFromErp { get; set; }

        public Characteristic Colour { get; set; }

        public Characteristic Size { get; set; }

        public Characteristic Style { get; set; }

        public Characteristic Quality { get; set; }

        public Assortment Assortment { get; set; }

        public PrimeCargoIntegration PrimeCargoIntegration { get; set; }
    }
}
