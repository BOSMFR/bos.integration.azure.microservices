using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class PrimeCargoPurchaseLineResponseDTO
    {
        [XmlElement("goodsReceivalLineId")]
        public int? GoodsReceivalLineId { get; set; }

        [XmlElement("extReference")]
        public int ExtReference { get; set; }

        [XmlElement("qty")]
        public int Qty { get; set; }

        [XmlElement("productId")]
        public int ProductId { get; set; }

        [XmlElement("reasonCode")]
        public string ReasonCode { get; set; }

        [XmlElement("customsReference")]
        public string CustomsReference { get; set; }

        [XmlElement("productBarcodeId")]
        public int? ProductBarcodeId { get; set; }

        [XmlElement("batchExpected")]
        public string BatchExpected { get; set; }

        [XmlElement("bestBeforeExpected")]
        public string BestBeforeExpected { get; set; }

        [XmlElement("addedManually")]
        public bool? AddedManually { get; set; }

        [XmlElement("amountReceived")]
        public int? AmountReceived { get; set; }

        [XmlElement("lastReceived")]
        public string LastReceived { get; set; }
    }
}
