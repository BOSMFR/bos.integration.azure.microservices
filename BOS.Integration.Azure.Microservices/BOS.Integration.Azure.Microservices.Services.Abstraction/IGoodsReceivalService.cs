using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IGoodsReceivalService
    {
        Task<ActionExecutionResult> CreateGoodsReceivalAsync(GoodsReceivalDTO goodsReceivalDTO);

        Task<bool> UpdateGoodsReceivalFromPrimeCargoInfoAsync(PrimeCargoGoodsReceivalResponseDTO primeCargoResponseObject);

        Task<List<GoodsReceival>> GetGoodsReceivalsByFilterAsync(GoodsReceivalFilterDTO goodsReceivalFilter);
    }
}
