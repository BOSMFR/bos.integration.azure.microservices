using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival
{
    public class GoodsReceival : ErpEntity
    {
        public string DocumentType { get; set; }

        public string No { get; set; }

        public string ShipToCode { get; set; }

        public string ShipToName { get; set; }

        public string ShipToName2 { get; set; }

        public string ShipToAddress { get; set; }

        public string ShipToAddress2 { get; set; }

        public string ShipToCity { get; set; }

        public string TransportMethod { get; set; }

        public string ShipToPostCode { get; set; }

        public string Eta { get; set; }

        public bool EtaConfirmed { get; set; }

        public string ErpRecordEvent { get; set; }

        public List<PurchaseLine> PurchaseLines { get; set; }

        public PrimeCargoGoodsReceivalResponseDTO PrimeCargoData { get; set; }
    }
}
