namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class ErpMessageStatus
    {
        // General
        public const string ReceivedFromErp = "Received from ERP";

        // Sku
        public const string UpdateMessage = "Update Message added to Azure Service bus";
        public const string CreateMessage = "Create Message added to Azure Service bus";
        public const string DeliveredSuccessfully = "Delivered Successfully to Prime Cargo";
        public const string Error = "Data Validation Error";
        public const string CreateTimeout = "Timeout received from Prime Cargo API (Create)";
        public const string UpdateTimeout = "Timeout received from Prime Cargo API (Update)";

        // Collection
        public const string CollectionUpdatedSuccessfully = "Collection successfully updated into Plytix";
        public const string CollectionUpdateError = "Error during updating Collection into Plytix";
    }
}
