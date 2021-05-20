namespace BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder
{
    public class SalesLine
    {
        public int ExtReference { get; set; }

        public string ProductId { get; set; }

        public double Qty { get; set; }

        public string CustomsReference { get; set; }

        public string CustomerId1 { get; set; }

        public string CustomerId2 { get; set; }

        public SalesLineProperty<string> Proporties { get; set; }
    }
}
