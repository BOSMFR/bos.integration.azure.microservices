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
        public const string ErrorValidation = " validation error";

        // SSIS
        public const string SsisMessageReceived = "SSIS message Received";

        // Packshot
        public const string ErrorCreatingPackshot = "Error creating a Packshot - ";
        public const string PackshotWrongImageFormat = "The packshot has the wrong image format";
        public const string PackshotBrandMissed = "The packshot product brand value is missed";
        public const string PackshotCreateMessageSentServiceBus = "Packshot Create Message Sent to Azure Service Bus";
        public const string PackshotUpdateMessageSentServiceBus = "Packshot Update Message Sent to Azure Service Bus";
        public const string ErrorCreatePackshotPlytix = "Error creating a Packshot in Plytix - ";
        public const string ErrorUpdatePackshotPlytix = "Error updating a Packshot in Plytix - ";
        public const string ErrorUpdatePackshotCategoryPlytix = "Error during updating Packshot categories into Plytix - ";
        public const string SuccsessUpdatePackshotCategoryPlytix = "Packshot categories successfully updated into Plytix";

        // PrimeCargo
        public const string PrimeCargoCreateMessageSentServiceBus = " Create Message Sent to Azure Service Bus";
        public const string PrimeCargoUpdateMessageSentServiceBus = " Update Message Sent to Azure Service Bus";
        public const string PrimeCargoSuccessfullyReceived = " is successfully received from Prime Cargo";
        public const string PrimeCargoErrorGetting = "Error getting a {0} from Prime Cargo - ";

        // Sku
        public const string PreparingMessageCanceled = "Preparing message canceled - startDatePrimeCargoExport set to ";

        // GoodsReceival
        public const string ErrorCreatingGoodsReceival = "Error creating a GoodsReceival - ";
        public const string SuccessfullyUpdatedGoodsReceival = "The GoodsReceival is successfully updated into Nav";
        public const string ErrorUpdatingGoodsReceival = "Error updating a GoodsReceival into Nav - ";
        public const string SuccessfullyClosedGoodsReceival = "The GoodsReceival is successfully closed into Nav";
        public const string ErrorClosingGoodsReceival = "Error closing a GoodsReceival into Nav - ";

        // PickOrder
        public const string ErrorCreatingPickOrder = "Error creating a PickOrder - ";
        public const string SuccessfullyUpdatedPickOrder = "The PickOrder is successfully updated into Nav";
        public const string ErrorUpdatingPickOrder = "Error updating a PickOrder into Nav - ";
        public const string SuccessfullyClosedPickOrder = "The PickOrder is successfully closed into Nav";
        public const string ErrorClosingPickOrder = "Error closing a PickOrder into Nav - ";

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
        public const string PlytixInstanceError = "Could not find any active Plytix instances";
        public const string PlytixTokenError = "Could not get access token for instance - ";
        public const string PlytixRequestTimeOut = "TimeOut sending request to Plytix API";
        public const string PlytixDeliveredSuccessfully = "Delivered Successfully to Plytix API";
    }
}
