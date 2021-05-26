namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public class NavSettings
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public ErpSettings Sku { get; set; }

        public ErpSettings GoodsReceival { get; set; }

        public ErpSettings PickOrder { get; set; }
    }
}
