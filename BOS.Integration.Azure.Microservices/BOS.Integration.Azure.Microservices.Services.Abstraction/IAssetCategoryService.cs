using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IAssetCategoryService
    {
        Task<List<string>> GetAssetCategoryIdsByPackshotInfoAsync(PlytixPackshotUpdateCategoryDTO plytixPackshot);
    }
}
