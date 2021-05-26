using BOS.Integration.Azure.Microservices.Domain.ValidationAttributes;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder
{
    public class SalesLineProperty<T>
    {
        public double CostPrice { get; set; }

        public double SalesPrice { get; set; }

        public string Variant1 { get; set; }

        public string Variant2 { get; set; }

        public string Variant3 { get; set; }

        public string Variant4 { get; set; }

        public string Variant5 { get; set; }

        public string CostCurrencyCode { get; set; }

        public string SalesCurrencyCode { get; set; }

        public string OriginCountryAlpha { get; set; }

        public string MeasureCode { get; set; }

        [IsInteger64Validation]
        public T TariffNumber { get; set; }

        public double NetWeight { get; set; }

        public double GrossWeight { get; set; }

        public string Gender { get; set; }

        public string MaterialComposition { get; set; }

        public string FabricMarked { get; set; }

        public string FabricMultilated { get; set; }

        public string Manufacturer { get; set; }

        public string ManufacturerAddress { get; set; }

        public string ManufacturerZipCode { get; set; }

        public string ManufacturerCity { get; set; }
    }
}
