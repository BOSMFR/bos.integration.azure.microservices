namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class ErpMessageStatus
    {
        // General
        public const string ReceivedFromErp = "Received from ERP";
        public const string ReceivedFromSsis = "Received from SSIS";

        // Sku, GoodsReceival
        public const string UpdateMessage = "Update Message added to Azure Service bus";
        public const string CreateMessage = "Create Message added to Azure Service bus";
        public const string DeliveredSuccessfully = "Delivered Successfully to Prime Cargo";
        public const string Error = "Data Validation Error";
        public const string CreateTimeout = "Timeout received from Prime Cargo API (Create)";
        public const string UpdateTimeout = "Timeout received from Prime Cargo API (Update)";

        // Collection
        public const string CollectionUpdatedSuccessfully = "Collection successfully updated into Plytix";
        public const string CollectionUpdateError = "Error during updating Collection into Plytix";

        // Delivery period
        public const string DeliveryPeriodUpdatedSuccessfully = "Delivery period successfully updated into Plytix";
        public const string DeliveryPeriodUpdateError = "Error during updating Delivery period into Plytix";

        // Plytix
        public const string PlytixSyncSuccessfully = "Synchronized with Plytix successfully";
        public const string PlytixSyncError = "Error during synchronization with Plytix";
        public const string PlytixCreateTimeout = "Timeout received from Plytix API (Create)";
        public const string PlytixUpdateTimeout = "Timeout received from Plytix API (Update)";
        public const string PlytixDeliveredSuccessfully = "Delivered Successfully to Plytix";

        // Packshot 
        public const string ErrorCreatePackshot = "Error during creating Packshot into Plytix";
        public const string ErrorUpdatePackshot = "Error during updating Packshot into Plytix";
        public const string SuccessfullyUpdatedPackshotCategory = "Packshot categories successfully updated into Plytix";
        public const string ErrorUpdatedPackshotCategory = "Error during updating Packshot categories into Plytix";
    }
}
