namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class ErpMessageStatus
    {
        public const string ReceivedFromErp = "Received from ERP";
        public const string UpdateMessage = "Update Message added to Azure Service bus";
        public const string CreateMessage = "Create Message added to Azure Service bus";
        public const string DeliveredSuccessfully = "Delivered Successfully to Prime Cargo";
        public const string Error = "Data Validation Error";
        public const string CreateTimeout = "Timeout received from Prime Cargo API (Create)";
        public const string UpdateTimeout = "Timeout received from Prime Cargo API (Update)";
    }
}
