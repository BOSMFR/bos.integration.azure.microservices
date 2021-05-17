using System.Collections.Generic;
using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    [XmlRoot(ElementName = "root")]
    public class PrimeCargoGoodsReceivalResponseDTO
    {
        [XmlElement("goodsReceivalId")]
        public int? GoodsReceivalId { get; set; }

        [XmlElement("receivalNumber")]
        public string ReceivalNumber { get; set; }

        [XmlElement("receivalTypeId")]
        public int ReceivalTypeId { get; set; }

        [XmlElement("eta")]
        public string Eta { get; set; }

        [XmlElement("finished")]
        public bool? Finished { get; set; }

        [XmlElement("returnPickOrderNumber")]
        public string ReturnPickOrderNumber { get; set; }

        [XmlElement("createTime")]
        public string CreateTime { get; set; }

        [XmlElement("goodsReceiptStarted")]
        public string GoodsReceiptStarted { get; set; }

        [XmlElement("finishedTime")]
        public string FinishedTime { get; set; }

        [XmlElement("createTime_utc")]
        public string CreateTimeUtc { get; set; }

        [XmlElement("goodsReceiptStarted_UTC")]
        public string GoodsReceiptStartedUTC { get; set; }

        [XmlElement("finishedTime_utc")]
        public string FinishedTimeUtc { get; set; }

        [XmlElement("lines")]
        public List<PrimeCargoPurchaseLineResponseDTO> Lines { get; set; }
    }
}
