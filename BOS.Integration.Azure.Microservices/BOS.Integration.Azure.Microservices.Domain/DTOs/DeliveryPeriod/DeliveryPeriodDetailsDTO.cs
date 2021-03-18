namespace BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod
{
    public class DeliveryPeriodDetailsDTO
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string ShipmentPeriod { get; set; }

        public string ShipmentFrom { get; set; }

        public string ShipmentTo { get; set; }

        public string PlannedDeliveryDate { get; set; }

        public bool OtherPeriodsAllowed { get; set; }

        public bool UseOnOrderConfirmation { get; set; }

        public string ExpiryDate { get; set; }

        public bool Active { get; set; }
    }
}
