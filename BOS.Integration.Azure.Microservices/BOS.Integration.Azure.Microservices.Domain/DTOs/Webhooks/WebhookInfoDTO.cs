namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks
{
    public class WebhookInfoDTO
    {
        public string Type { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public string RequestBody { get; set; }
    }
}
