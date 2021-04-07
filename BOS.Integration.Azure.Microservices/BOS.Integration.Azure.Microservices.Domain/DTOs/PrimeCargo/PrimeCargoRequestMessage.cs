namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo
{
    public class PrimeCargoRequestMessage<T>
    {
        public LogInfo ErpInfo { get; set; }

        public T PrimeCargoRequestObject { get; set; }
    }
}
