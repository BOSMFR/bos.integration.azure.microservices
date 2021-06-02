using System;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PrimeCargoPickOrderCartonDTO
    {
        public int CartonId { get; set; }

        public int StandardCartonId { get; set; }

        public string Barcode { get; set; }

        public string CartonTrackingNumber { get; set; }

        public int PickOrderHeaderId { get; set; }

        public int CartonNumber { get; set; }

        public int Weight_g { get; set; }

        public int Width_mm { get; set; }

        public int Height_mm { get; set; }

        public int Depth_mm { get; set; }

        public DateTime CreatedTime { get; set; }

        public string ReturnCartonTrackingNumber { get; set; }

        public bool HoldCode { get; set; }

        public bool? HoldLabelPrinted { get; set; }

        public int ShipmentCSID { get; set; }

        public int? ReturnShipmentCSID { get; set; }

        public int? Volume_cm3 { get; set; }

        public string Waybill { get; set; }

        public List<PrimeCargoPickOrderStockPackedDTO> StockPackeds { get; set; }
    }
}
