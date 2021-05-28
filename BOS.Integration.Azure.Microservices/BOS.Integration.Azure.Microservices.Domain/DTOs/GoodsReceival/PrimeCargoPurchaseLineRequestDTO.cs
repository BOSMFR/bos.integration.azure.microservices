namespace BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival
{
    public class PrimeCargoPurchaseLineRequestDTO
    {
        public int ExtReference { get; set; }

        public int Qty { get; set; }

        public int ProductId { get; set; }

        public string CustomsReference { get; set; }
    }
}
