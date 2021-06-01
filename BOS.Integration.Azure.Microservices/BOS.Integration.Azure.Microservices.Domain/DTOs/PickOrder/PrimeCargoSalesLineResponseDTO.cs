using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoSalesLineResponseDTO
    {
        [XmlElement("pickOrderLineId")]
        public int? PickOrderLineId { get; set; }

        [XmlElement("extReference")]
        public string ExtReference { get; set; }

        [XmlElement("productId")]
        public int ProductId { get; set; }

        [XmlElement("qty")]
        public int Qty { get; set; }

        [XmlElement("partDescription")]
        public string PartDescription { get; set; }

        [XmlElement("cancelled")]
        public bool? Cancelled { get; set; }

        [XmlElement("closed")]
        public bool? Closed { get; set; }

        [XmlElement("foundQty")]
        public int? FoundQty { get; set; }

        [XmlElement("customsReference")]
        public string CustomsReference { get; set; }

        [XmlElement("lineCustomerId1")]
        public string CustomerId1 { get; set; }

        [XmlElement("lineCustomerId2")]
        public string CustomerId2 { get; set; }

        [XmlElement("lineCreatedTime")]
        public string LineCreatedTime { get; set; }

        [XmlElement("productBarcodeId")]
        public int? ProductBarcodeId { get; set; }

        [XmlElement("batchRequested")]
        public string BatchRequested { get; set; }

        [XmlElement("reasonCode")]
        public int? ReasonCode { get; set; }

        [XmlElement("lineProperties")]
        public PrimeCargoSalesLinePropertiesDTO Properties { get; set; }

        [XmlElement("packedQty")]
        public int? PackedQty { get; set; }
    }
}
