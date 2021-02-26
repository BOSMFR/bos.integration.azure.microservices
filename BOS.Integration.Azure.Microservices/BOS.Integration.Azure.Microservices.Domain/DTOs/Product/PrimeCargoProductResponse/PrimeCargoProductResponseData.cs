namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductResponseData
    {
        public string Barcode { get; set; }

        public int? ProductId { get; set; }

        public string PartNumber { get; set; }

        public int TypeId { get; set; }

        public string Description { get; set; }

        public string Variant1 { get; set; }

        public string Variant2 { get; set; }

        public string Variant3 { get; set; }

        public string Variant4 { get; set; }

        public string Variant5 { get; set; }

        public string CreatedTime { get; set; }

        public bool Fifo { get; set; }

        public bool GiveAway { get; set; }

        public bool ExtraService { get; set; }

        public string ErpjobId { get; set; }
    }
}
