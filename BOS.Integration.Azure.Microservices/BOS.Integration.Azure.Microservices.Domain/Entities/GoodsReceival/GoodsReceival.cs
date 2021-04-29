﻿using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival
{
    public class GoodsReceival : ErpEntity
    {
        public int DocumentType { get; set; }

        public string DocumentTypeText { get; set; }

        public string ReceiveListName { get; set; }

        public string OrderNo { get; set; }

        public string WmsDocumentNo { get; set; }

        public string WmsCustomsReference { get; set; }

        public string Eta { get; set; }

        public string ErpRecordEvent { get; set; }

        public List<PurchaseLine> PurchaseLines { get; set; }

        public PrimeCargoGoodsReceivalResponseDTO PrimeCargoData { get; set; }
    }
}
