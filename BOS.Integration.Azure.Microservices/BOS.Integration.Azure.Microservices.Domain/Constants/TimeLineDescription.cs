namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class TimeLineDescription
    {
        // General
        public const string ErpMessageReceived = "ERP message Received";
        public const string DataValidationFailed = "Data validation failed";
        public const string PrepareForServiceBus = "Prepare message for Azure Service Bus";
        public const string DeliveredSuccessfullyToPrimeCargo = "Delivered Successfully to Prime Cargo Product API";
        public const string PrimeCargoRequestTimeOut = "TimeOut sending request to Prime Cargo Product API";
        public const string PrimeCargoRequestError = "Error sending request to Prime Cargo Product API - ";
        public const string ErpUpdatedSuccessfully = "ERP updated Successfully";
        public const string TimeOutUpdatingErp = "TimeOut updating ERP";
        public const string ErrorUpdatingERP = "Error updating ERP - ";

        // Sku
        public const string PreparingMessageCanceled = "Preparing message canceled - startDatePrimeCargoExport set to ";
        public const string ProductCreateMessageSentServiceBus = "Product Create Message Sent to Azure Service Bus";
        public const string ProductUpdateMessageSentServiceBus = "Product Update Message Sent to Azure Service Bus";

        // GoodsReceival
        public const string ErrorCreatingGoodsReceival = "Error creating a GoodsReceival - ";
        public const string ErrorGoodsReceivalType = "Error mapping GoodsReceival document type";
        public const string GoodsReceivalCreateMessageSentServiceBus = "GoodsReceival Create Message Sent to Azure Service Bus";
        public const string GoodsReceivalUpdateMessageSentServiceBus = "GoodsReceival Update Message Sent to Azure Service Bus";

        // Collection
        public const string CollectionPaUpdatedSuccessfully = "Collection product attribute successfully updated into Plytix";
        public const string CollectionPaUpdateError = "Error during updating Collection product attribute into Plytix - ";
        public const string CollectionAcUpdatedSuccessfully = "Collection asset category successfully updated into Plytix";
        public const string CollectionAcUpdateError = "Error during updating Collection asset category into Plytix - ";

        // Delivery period
        public const string DeliveryPeriodPaUpdatedSuccessfully = "DeliveryPeriod product attribute successfully updated into Plytix";
        public const string DeliveryPeriodPaUpdateError = "Error during updating DeliveryPeriod product attribute into Plytix - ";
        public const string DeliveryPeriodAcUpdatedSuccessfully = "Delivery Period asset category successfully updated into Plytix";
        public const string DeliveryPeriodAcUpdateError = "Error during updating Delivery Period asset category into Plytix - ";

        // Plytix
        public const string PlytixSyncSuccessfully = "Synchronized with Plytix successfully. Synchronization made by user: ";
        public const string PlytixSyncError = "Error during synchronization with Plytix. Synchronization made by user: ";
    }
}
