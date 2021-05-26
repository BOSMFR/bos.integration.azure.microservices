using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoPickOrderPropertiesDTO
    {
        [XmlElement("pickOrderHeaderId")]
        public int? PickOrderHeaderId { get; set; }

        [XmlElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [XmlElement("invoiceDate")]
        public string InvoiceDate { get; set; }

        [XmlElement("freightCharges")]
        public double? FreightCharges { get; set; }

        [XmlElement("insurance")]
        public bool? Insurance { get; set; }

        [XmlElement("chargesValue")]
        public int? ChargesValue { get; set; }

        [XmlElement("chargesDescription")]
        public string ChargesDescription { get; set; }

        [XmlElement("addComment")]
        public string AddComment { get; set; }

        [XmlElement("declarationStatement")]
        public string DeclarationStatement { get; set; }

        [XmlElement("incoterm")]
        public string Incoterm { get; set; }

        [XmlElement("vatSender")]
        public string VatSender { get; set; }

        [XmlElement("vatReceiver")]
        public string VatReceiver { get; set; }

        [XmlElement("vatBuyer")]
        public string VatBuyer { get; set; }
    }
}
