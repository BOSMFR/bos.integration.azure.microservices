using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder
{
    public class PickOrder : ErpEntity
    {
        private const int InstructionMaxLength = 2;


        public string OrderNumber { get; set; }

        public int OrderTypeId { get; set; }

        public string CountryId { get; set; }

        [IsInteger32Validation]
        public string CountryIsoCode { get; set; }

        [IsInteger32Validation]
        public string ShippingProductId { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverAddress1 { get; set; }

        public string ReceiverAddress2 { get; set; }

        public string ReceiverContactPerson { get; set; }

        public string ReceiverPhoneFax { get; set; }

        public string ReceiverEmail { get; set; }

        public string ReceiverZip { get; set; }

        public string ReceiverCity { get; set; }

        [IsNullableInteger32Validation]
        public string UsStateId { get; set; }

        public string CustomerNumber { get; set; }

        public string CustomerID1 { get; set; }

        public string CustomerID2 { get; set; }

        public string CustomerID3 { get; set; }

        public string ShipDate { get; set; }

        public bool HoldCode { get; set; }

        public string HoldReason { get; set; }

        public int LcId { get; set; }

        public int ShopId { get; set; }

        public string ShippingProductCodeParameters { get; set; }

        public string ShipmentInstructions { get; set; }

        [MaxLength(InstructionMaxLength)]
        public string PickingInstruction { get; set; }

        [MaxLength(InstructionMaxLength)]
        public string PackingInstruction { get; set; }

        [IsNullableInteger32Validation]
        public string SubOwnerId { get; set; }

        [IsNullableInteger32Validation]
        public string SubOwnerAddressId { get; set; }

        public string ShippingInstructionDriver { get; set; }

        public string ShippingInstructionReceiver { get; set; }

        public int CustomsStatus { get; set; }

        public string BusinessType { get; set; }

        public string BuyerName { get; set; }

        public string BuyerAddress { get; set; }

        public string BuyerZip { get; set; }

        public string BuyerCity { get; set; }

        public string BuyerCountry { get; set; }

        public string BuyerState { get; set; }

        public string ErpRecordEvent { get; set; }

        public bool IsClosed { get; set; }

        public List<SalesLine> SalesLines { get; set; }

        public PrimeCargoPickOrderResponseDTO PrimeCargoData { get; set; }
    }
}
