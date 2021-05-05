using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class GoodsReceivalService : IGoodsReceivalService
    {
        private readonly IGoodsReceivalRepository repository;
        private readonly IMapper mapper;

        public GoodsReceivalService(IGoodsReceivalRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<ActionExecutionResult> CreateGoodsReceivalAsync(GoodsReceivalDTO goodsReceivalDTO)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newGoodsReceival = this.mapper.Map<GoodsReceival>(goodsReceivalDTO);

                var goodsReceival = await repository.GetByIdAsync(newGoodsReceival.WmsDocumentNo, NavObjectCategory.GoodsReceival);

                if (goodsReceival == null)
                {
                    newGoodsReceival.Category = NavObjectCategory.GoodsReceival;
                    newGoodsReceival.ReceivedFromErp = DateTime.Now;

                    await repository.AddAsync(newGoodsReceival, newGoodsReceival.Category);

                    actionResult.Entity = newGoodsReceival;
                    actionResult.Succeeded = true;
                }
                else
                {
                    actionResult.Entity = goodsReceival;
                    actionResult.Error = $"The GoodsReceival with 'WmsDocumentNo' = {goodsReceival.WmsDocumentNo} already exists";
                }

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<bool> UpdateGoodsReceivalFromPrimeCargoInfoAsync(PrimeCargoGoodsReceivalResponseDTO primeCargoResponseObject)
        {
            var goodsReceival = await repository.GetByIdAsync(primeCargoResponseObject.ReceivalNumber, NavObjectCategory.GoodsReceival);

            if (goodsReceival == null)
            {
                return false;
            }

            goodsReceival.PrimeCargoData = primeCargoResponseObject;

            await repository.UpdateAsync(goodsReceival, goodsReceival.Category);

            return true;
        }
    }
}
