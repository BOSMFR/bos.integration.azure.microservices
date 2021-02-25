namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class LogInfo
    {
        public string Object { get; set; }

        public string ObjectId { get; set; }

        public string ErpjobId { get; set; }

        public string ErpDateTime { get; set; }

        public string ReceivedFromErp { get; set; }
    }
}
