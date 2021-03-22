namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class TimeLineDescription
    {
        // General
        public const string ErpMessageReceived = "ERP message Received";
        public const string DataValidationFailed = "Data validation failed";

        // Sku
        public const string PrepareForServiceBus = "Prepare message for Azure Service Bus";
        public const string PreparingMessageCanceled = "Preparing message canceled - startDatePrimeCargoExport set to ";
        public const string ProductCreateMessageSentServiceBus = "Product Create Message Sent to Azure Service Bus";
        public const string ProductUpdateMessageSentServiceBus = "Product Update Message Sent to Azure Service Bus";
        public const string DeliveredSuccessfullyToPrimeCargo = "Delivered Successfully to Prime Cargo Product API";
        public const string PrimeCargoRequestTimeOut = "TimeOut sending request to Prime Cargo Product API";
        public const string PrimeCargoRequestError = "Error sending request to Prime Cargo Product API - ";
        public const string ErpUpdatedSuccessfully = "ERP updated Successfully";
        public const string TimeOutUpdatingErp = "TimeOut updating ERP";
        public const string ErrorUpdatingERP = "Error updating ERP - ";

        // Collection
        public const string CollectionUpdatedSuccessfully = "Collection successfully updated into Plytix";
        public const string CollectionUpdateError = "Error during updating Collection into Plytix - ";

        // Delivery period
        public const string DeliveryPeriodUpdatedSuccessfully = "Delivery period successfully updated into Plytix";
        public const string DeliveryPeriodUpdateError = "Error during updating Delivery period into Plytix - ";
    }
}
