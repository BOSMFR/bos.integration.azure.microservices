namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Collection
{
    public class CollectionDetailsDTO
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string ShipmentPeriod { get; set; }

        public string ShipmentFrom { get; set; }

        public string ShipmentToDate { get; set; }

        public string PlannedDelivery { get; set; }

        public string ExpiryDate { get; set; }

        public string StartingDate { get; set; }

        public string EndingDate { get; set; }

        public string BudgetCompareSeasonCode { get; set; }

        public bool ShowExternal { get; set; }
    }
}
