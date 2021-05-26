using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoSalesLinePropertiesDTO
    {
        [XmlElement("variant1")]
        public string Variant1 { get; set; }

        [XmlElement("variant2")]
        public string Variant2 { get; set; }

        [XmlElement("variant3")]
        public string Variant3 { get; set; }

        [XmlElement("variant4")]
        public string Variant4 { get; set; }

        [XmlElement("variant5")]
        public string Variant5 { get; set; }

        [XmlElement("salesPrice")]
        public double SalesPrice { get; set; }

        [XmlElement("salesCurrencyCode")]
        public string SalesCurrencyCode { get; set; }

        [XmlElement("costPrice")]
        public double CostPrice { get; set; }

        [XmlElement("costCurrencyCode")]
        public string CostCurrencyCode { get; set; }

        [XmlElement("originCountryAlpha")]
        public string OriginCountryAlpha { get; set; }

        [XmlElement("measureCode")]
        public string MeasureCode { get; set; }

        [XmlElement("tariffNumber")]
        public long TariffNumber { get; set; }

        [XmlElement("netWeight")]
        public double NetWeight { get; set; }

        [XmlElement("grossWeight")]
        public double GrossWeight { get; set; }

        [XmlElement("gender")]
        public string Gender { get; set; }

        [XmlElement("materialComposition")]
        public string MaterialComposition { get; set; }

        [XmlElement("construction")]
        public string Construction { get; set; }

        [XmlElement("fabricMarked")]
        public string FabricMarked { get; set; }

        [XmlElement("fabricMutilated")]
        public string FabricMutilated { get; set; }

        [XmlElement("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlElement("manufacturerAddress")]
        public string ManufacturerAddress { get; set; }

        [XmlElement("manufacturerZipCode")]
        public string ManufacturerZipCode { get; set; }

        [XmlElement("manufacturerCity")]
        public string ManufacturerCity { get; set; }

        [XmlElement("wrapperDataBag")]
        public string WrapperDataBag { get; set; }

        [XmlElement("wrapperPrintedTime")]
        public string WrapperPrintedTime { get; set; }
    }
}
