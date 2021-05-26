using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
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
                    newGoodsReceival.ReceivedFromErp = DateTime.UtcNow;

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

        public async Task<ActionExecutionResult> CreateGoodsReceivalFromPrimeCargoInfoAsync(PrimeCargoGoodsReceivalResponseDTO primeCargoResponseObject)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newGoodsReceival = new GoodsReceival();

                newGoodsReceival.PrimeCargoData = primeCargoResponseObject;
                newGoodsReceival.WmsDocumentNo = newGoodsReceival.PrimeCargoData.ReceivalNumber;

                newGoodsReceival.Category = NavObjectCategory.GoodsReceival;
                newGoodsReceival.ReceivedFromErp = DateTime.UtcNow;

                await repository.AddAsync(newGoodsReceival, newGoodsReceival.Category);

                actionResult.Entity = newGoodsReceival;
                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<bool> UpdateGoodsReceivalFromPrimeCargoInfoAsync(PrimeCargoGoodsReceivalResponseDTO primeCargoResponseObject, GoodsReceival goodsReceival = null)
        {
            goodsReceival ??= await repository.GetByIdAsync(primeCargoResponseObject.ReceivalNumber, NavObjectCategory.GoodsReceival);

            if (goodsReceival == null)
            {
                return false;
            }

            goodsReceival.PrimeCargoData = primeCargoResponseObject;

            await repository.UpdateAsync(goodsReceival, goodsReceival.Category);

            return true;
        }

        public async Task<List<GoodsReceival>> GetGoodsReceivalsByFilterAsync(GoodsReceivalFilterDTO goodsReceivalFilter)
        {
            goodsReceivalFilter.FromDate ??= DateTime.MinValue;
            goodsReceivalFilter.ToDate = goodsReceivalFilter.ToDate.HasValue ? goodsReceivalFilter.ToDate.Value.AddDays(1) : DateTime.MaxValue;

            return await this.repository.GetByFilterAsync(goodsReceivalFilter, NavObjectCategory.GoodsReceival);
        }

        public Task<GoodsReceival> GetGoodsReceivalByIdAsync(string id)
        {
            return repository.GetByIdAsync(id, NavObjectCategory.GoodsReceival);
        }

        public async Task<bool> SetGoodsReceivalClosedAsync(GoodsReceival goodsReceival)
        {
            try
            {
                goodsReceival.IsClosed = true;

                await repository.UpdateAsync(goodsReceival, goodsReceival.Category);

                return true;
            }
            catch
            {
                return false;
            }            
        }
    }
}
