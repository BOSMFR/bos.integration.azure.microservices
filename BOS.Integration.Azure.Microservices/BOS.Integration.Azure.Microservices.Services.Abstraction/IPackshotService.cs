using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPackshotService
    {
        Task<ActionExecutionResult> CreatePackshotAsync(PackshotDTO packshotDTO);

        Task<bool> UpdatePackshotFromPlytixInfoAsync(string packshotId, PlytixData<PlytixPackshotResponseData> plytixResponseObject);

        Task<List<Packshot>> GetPackshotsByFilterAsync(PackshotFilterDTO packshotFilter);
    }
}
