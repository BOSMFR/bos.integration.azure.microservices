namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class RequestMessage<T>
    {
        public LogInfo ErpInfo { get; set; }

        public T RequestObject { get; set; }
    }
}
