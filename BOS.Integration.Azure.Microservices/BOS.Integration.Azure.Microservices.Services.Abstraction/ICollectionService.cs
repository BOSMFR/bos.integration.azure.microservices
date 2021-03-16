using BOS.Integration.Azure.Microservices.Domain.DTOs.Collection;
using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface ICollectionService
    {
        Task<CollectionEntity> CreateOrUpdateCollectionAsync(CollectionDTO productDTO);
    }
}
