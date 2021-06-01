using System.Collections.Generic;
using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    [XmlRoot(ElementName = "root")]
    public class PrimeCargoPickOrderResponseDTO
    {
        [XmlElement("pickOrderHeaderId")]
        public int? PickOrderHeaderId { get; set; }

        [XmlElement("orderNumber")]
        public string OrderNumber { get; set; }

        [XmlElement("orderTypeId")]
        public int OrderTypeId { get; set; }

        [XmlElement("countryId")]
        public int CountryId { get; set; }

        [XmlElement("statusId")]
        public int StatusId { get; set; }

        [XmlElement("shippingProductId")]
        public int ShippingProductId { get; set; }

        [XmlElement("receiverName")]
        public string ReceiverName { get; set; }

        [XmlElement("receiverAddress1")]
        public string ReceiverAddress1 { get; set; }

        [XmlElement("receiverAddress2")]
        public string ReceiverAddress2 { get; set; }

        [XmlElement("receiverContactPerson")]
        public string ReceiverContactPerson { get; set; }

        [XmlElement("receiverPhoneFax")]
        public string ReceiverPhoneFax { get; set; }

        [XmlElement("receiverEmail")]
        public string ReceiverEmail { get; set; }

        [XmlElement("receiverZip")]
        public string ReceiverZip { get; set; }

        [XmlElement("receiverCity")]
        public string ReceiverCity { get; set; }

        [XmlElement("customerNumber")]
        public string CustomerNumber { get; set; }

        [XmlElement("customerId1")]
        public string CustomerId1 { get; set; }

        [XmlElement("customerId2")]
        public string CustomerId2 { get; set; }

        [XmlElement("customerId3")]
        public string CustomerId3 { get; set; }

        [XmlElement("shipDate")]
        public string ShipDate { get; set; }

        [XmlElement("fileReceiveTime")]
        public string FileReceiveTime { get; set; }

        [XmlElement("triedReleasedTime")]
        public string TriedReleasedTime { get; set; }

        [XmlElement("releasedTime")]
        public string ReleasedTime { get; set; }

        [XmlElement("closedTime")]
        public string ClosedTime { get; set; }

        [XmlElement("packingInformation")]
        public string PackingInformation { get; set; }

        [XmlElement("holdCode")]
        public bool? HoldCode { get; set; }

        [XmlElement("holdReason")]
        public string HoldReason { get; set; }

        [XmlElement("lcid")]
        public int Lcid { get; set; }

        [XmlElement("shopId")]
        public int ShopId { get; set; }

        [XmlElement("shippingProductCodeParameters")]
        public string ShippingProductCodeParameters { get; set; }

        [XmlElement("shipmentInstructions")]
        public string ShipmentInstructions { get; set; }

        [XmlElement("pickingInstruction")]
        public string PickingInstruction { get; set; }

        [XmlElement("packingInstruction")]
        public string PackingInstruction { get; set; }

        [XmlElement("collectName")]
        public string CollectName { get; set; }

        [XmlElement("collectAddress")]
        public string CollectAddress { get; set; }

        [XmlElement("collectZip")]
        public string CollectZip { get; set; }

        [XmlElement("collectCity")]
        public string CollectCity { get; set; }

        [XmlElement("collectCountryId")]
        public int? CollectCountryId { get; set; }

        [XmlElement("subOwnerId")]
        public int? SubOwnerId { get; set; }

        [XmlElement("subOwnerAddressId")]
        public int? SubOwnerAddressId { get; set; }

        [XmlElement("shippingInstructionDriver")]
        public string ShippingInstructionDriver { get; set; }

        [XmlElement("shippingInstructionReceiver")]
        public string ShippingInstructionReceiver { get; set; }

        [XmlElement("usStateId")]
        public int? UsStateId { get; set; }

        [XmlElement("customsStatus")]
        public int CustomsStatus { get; set; }

        [XmlElement("ssccLabel")]
        public int? SsccLabel { get; set; }

        [XmlElement("useMultiplePackageShipment")]
        public bool UseMultiplePackageShipment { get; set; }

        [XmlElement("pickStartedInAutostoreTime")]
        public string PickStartedInAutostoreTime { get; set; }

        [XmlElement("pickFinishedInAutostoreTime")]
        public string PickFinishedInAutostoreTime { get; set; }

        [XmlElement("sentToAutostore")]
        public bool? SentToAutostore { get; set; }

        [XmlElement("priceTag")]
        public bool? PriceTag { get; set; }

        [XmlElement("businessType")]
        public string BusinessType { get; set; }

        [XmlElement("buyerName")]
        public string BuyerName { get; set; }

        [XmlElement("buyerAddress")]
        public string BuyerAddress { get; set; }

        [XmlElement("buyerZip")]
        public string BuyerZip { get; set; }

        [XmlElement("buyerCity")]
        public string BuyerCity { get; set; }

        [XmlElement("buyerCountry")]
        public string BuyerCountry { get; set; }

        [XmlElement("buyerState")]
        public string BuyerState { get; set; }

        [XmlElement("groupType")]
        public string GroupType { get; set; }

        [XmlElement("properties")]
        public PrimeCargoPickOrderPropertiesDTO Properties { get; set; }

        [XmlElement("lines")]
        public List<PrimeCargoSalesLineResponseDTO> Lines { get; set; }
    }
}
