using BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoSalesLineRequestDTO
    {
        public string ExtReference { get; set; }

        public int ProductId { get; set; }

        public int Qty { get; set; }

        public string CustomsReference { get; set; }

        public string CustomerId1 { get; set; }

        public string CustomerId2 { get; set; }

        public SalesLineProperty<long> Properties { get; set; }
    }
}
