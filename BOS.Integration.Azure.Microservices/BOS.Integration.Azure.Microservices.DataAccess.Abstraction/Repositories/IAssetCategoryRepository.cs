using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IAssetCategoryRepository : IRepository<AssetCategory>
    {
        Task<AssetCategory> GetByNameAsync(string name, string partitionKey = null);
    }
}
