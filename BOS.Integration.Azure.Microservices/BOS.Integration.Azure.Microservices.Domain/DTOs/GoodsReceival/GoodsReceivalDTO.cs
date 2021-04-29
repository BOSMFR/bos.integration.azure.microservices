using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class GoodsReceivalDTO
    {

        public int DocumentType { get; set; }

        public string DocumentTypeText { get; set; }

        public string ReceiveListName { get; set; }

        public string OrderNo { get; set; }

        public string WmsDocumentNo { get; set; }

        public string WmsCustomsReference { get; set; }

        public string Eta { get; set; }

        public string ErpRecordEvent { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public List<PurchaseLine> PurchaseLines { get; set; }
    }
}
