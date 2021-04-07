using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival
{
    public class PurchaseLine
    {
        public int LineNo { get; set; }

        public string No { get; set; }

        public string LocationCode { get; set; }

        public double QtyToReceive { get; set; }

        public double QuantityReceived { get; set; }

        public string AssortmentCode { get; set; }

        public double AssortmentQuantity { get; set; }

        public string AssortmentColour { get; set; }

        public string AssortmentColourDescription { get; set; }

        public string AssortmentStyle { get; set; }

        public List<PurchaseVariant> PurchaseVariant { get; set; }
    }
}
