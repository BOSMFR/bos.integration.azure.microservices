namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class HttpResponse
    {
        public string StatusCode { get; set; }

        public string Error { get; set; }

        public string ErrorObject { get; set; }
    }
}
