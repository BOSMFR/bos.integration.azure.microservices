namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo
{
    public class PrimeCargoResponseMessage<T>
    {
        public LogInfo ErpInfo { get; set; }

        public T PrimeCargoResponseObject { get; set; }
    }
}
