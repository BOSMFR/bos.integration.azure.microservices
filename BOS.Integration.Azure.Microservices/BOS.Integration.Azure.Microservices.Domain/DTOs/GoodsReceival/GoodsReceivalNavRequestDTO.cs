namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class GoodsReceivalNavRequestDTO
    {
        public string WMSDocumentNo { get; set; }

        public int VMSDocumentLineNo { get; set; }

        public int WMSGoodsReceivalId { get; set; }

        public int WMSGoodsReceivalLineId { get; set; }
    }
}
