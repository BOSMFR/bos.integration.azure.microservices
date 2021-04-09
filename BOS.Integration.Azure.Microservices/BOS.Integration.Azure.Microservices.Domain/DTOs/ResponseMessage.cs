namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class ResponseMessage<T>
    {
        public LogInfo ErpInfo { get; set; }

        public T ResponseObject { get; set; }
    }
}
