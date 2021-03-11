namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Auth
{
    public class PrimeCargoAuthResponseDTO
    {
        public PrimeCargoAuthData Data { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
