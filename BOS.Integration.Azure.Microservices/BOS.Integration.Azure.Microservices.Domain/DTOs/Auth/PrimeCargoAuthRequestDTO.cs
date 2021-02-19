namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Auth
{
    public class PrimeCargoAuthRequestDTO
    {
        public string OwnerCode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
