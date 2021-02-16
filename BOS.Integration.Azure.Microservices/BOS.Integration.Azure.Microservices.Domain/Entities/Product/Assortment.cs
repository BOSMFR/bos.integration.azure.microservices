namespace BOS.Integration.Azure.Microservices.Domain.Entities.Product
{
    public class Assortment
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public int Variant1 { get; set; }

        public string Variant1Text { get; set; }

        public int Variant2 { get; set; }

        public string Variant2Text { get; set; }

        public int Variant3 { get; set; }

        public string Variant3Text { get; set; }

        public int CrossVariant { get; set; }

        public bool Blocked { get; set; }

        public string TotalQty { get; set; }
    }
}
