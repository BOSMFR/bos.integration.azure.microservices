namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class PlytixSyncResultDTO
    {
        public ActionExecutionResult GeneralResult { get; set; }

        public ActionExecutionResult UpdateCollectionResult { get; set; }

        public ActionExecutionResult UpdateDeliveryPeriodResult { get; set; }
    }
}
