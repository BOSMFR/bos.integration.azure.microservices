using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoPickOrderRequestDTO
    {
        public string OrderNumber { get; set; }

        public int OrderTypeId { get; set; }

        public int CountryId { get; set; }

        public int ShippingProductId { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverAddress1 { get; set; }

        public string ReceiverAddress2 { get; set; }

        public string ReceiverContactPerson { get; set; }

        public string ReceiverPhoneFax { get; set; }

        public string ReceiverEmail { get; set; }

        public string ReceiverZip { get; set; }

        public string ReceiverCity { get; set; }

        public string CustomerNumber { get; set; }

        public string CustomerId1 { get; set; }

        public string CustomerId2 { get; set; }

        public string CustomerId3 { get; set; }

        public string ShipDate { get; set; }

        public bool HoldCode { get; set; }

        public string HoldReason { get; set; }

        public int Lcid { get; set; }

        public int ShopId { get; set; }

        public string ShippingProductCodeParameters { get; set; }

        public string ShipmentInstructions { get; set; }

        public string PickingInstruction { get; set; }

        public string PackingInstruction { get; set; }

        public int SubOwnerId { get; set; }

        public int SubOwnerAddressId { get; set; }

        public string ShippingInstructionDriver { get; set; }

        public string ShippingInstructionReceiver { get; set; }

        public int UsStateId { get; set; }

        public int CustomsStatus { get; set; }

        public string BusinessType { get; set; }

        public string BuyerName { get; set; }

        public string BuyerAddress { get; set; }

        public string BuyerZip { get; set; }

        public string BuyerCity { get; set; }

        public string BuyerCountry { get; set; }

        public string BuyerState { get; set; }

        public List<PrimeCargoSalesLineDTO> Lines { get; set; }
    }
}
