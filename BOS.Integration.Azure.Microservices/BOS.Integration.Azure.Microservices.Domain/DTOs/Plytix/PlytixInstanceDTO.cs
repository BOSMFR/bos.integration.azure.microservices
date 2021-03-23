using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class PlytixInstanceDTO
    {
        public PlytixInstance Plytix { get; set; }

        public string Token { get; set; }
    }
}
