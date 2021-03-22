using BOS.Integration.Azure.Microservices.Domain.DTOs.Noos;
using BOS.Integration.Azure.Microservices.Domain.Entities.Noos;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface INoosService
    {
        Task<Noos> CreateOrUpdateNoosAsync(NoosDTO noosDTO);
    }
}
