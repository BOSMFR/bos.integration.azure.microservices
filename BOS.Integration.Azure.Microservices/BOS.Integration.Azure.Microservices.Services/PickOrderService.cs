using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PickOrderService : IPickOrderService
    {
        private readonly IPickOrderRepository repository;
        private readonly IMapper mapper;

        public PickOrderService(IPickOrderRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<ActionExecutionResult> CreatePickOrderAsync(PickOrderDTO pickOrderDTO)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newPickOrder = this.mapper.Map<PickOrder>(pickOrderDTO);

                var pickOrder = await repository.GetByIdAsync(newPickOrder.OrderNumber, NavObjectCategory.PickOrder);

                if (pickOrder == null)
                {
                    newPickOrder.Category = NavObjectCategory.PickOrder;
                    newPickOrder.ReceivedFromErp = DateTime.UtcNow;

                    await repository.AddAsync(newPickOrder, newPickOrder.Category);

                    actionResult.Entity = newPickOrder;
                    actionResult.Succeeded = true;
                }
                else
                {
                    actionResult.Entity = pickOrder;
                    actionResult.Error = $"The PickOrder with 'OrderNumber' = {pickOrder.OrderNumber} already exists";
                }

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<ActionExecutionResult> CreatePickOrderFromPrimeCargoInfoAsync(PrimeCargoPickOrderResponseDTO primeCargoResponseObject)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newPickOrder = new PickOrder();

                newPickOrder.PrimeCargoData = primeCargoResponseObject;
                newPickOrder.OrderNumber = newPickOrder.PrimeCargoData.OrderNumber;

                newPickOrder.Category = NavObjectCategory.PickOrder;
                newPickOrder.ReceivedFromErp = DateTime.UtcNow;

                await repository.AddAsync(newPickOrder, newPickOrder.Category);

                actionResult.Entity = newPickOrder;
                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<bool> UpdatePickOrderFromPrimeCargoInfoAsync(PrimeCargoPickOrderResponseDTO primeCargoResponseObject, PickOrder pickOrder = null)
        {
            pickOrder ??= await repository.GetByIdAsync(primeCargoResponseObject.OrderNumber, NavObjectCategory.PickOrder);

            if (pickOrder == null)
            {
                return false;
            }

            pickOrder.PrimeCargoData = primeCargoResponseObject;

            await repository.UpdateAsync(pickOrder, pickOrder.Category);

            return true;
        }

        public async Task<List<PickOrder>> GetPickOrdersByFilterAsync(PickOrderFilterDTO pickOrderFilter)
        {
            pickOrderFilter.FromDate ??= DateTime.MinValue;
            pickOrderFilter.ToDate = pickOrderFilter.ToDate.HasValue ? pickOrderFilter.ToDate.Value.AddDays(1) : DateTime.MaxValue;

            return await this.repository.GetByFilterAsync(pickOrderFilter, NavObjectCategory.PickOrder);
        }

        public Task<List<PickOrder>> GetOpenPickOrdersAsync()
        {
            return repository.GetAllOpenAsync(NavObjectCategory.PickOrder);
        }

        public Task<PickOrder> GetPickOrderByIdAsync(string id)
        {
            return repository.GetByIdAsync(id, NavObjectCategory.PickOrder);
        }

        public async Task<bool> SetPickOrderClosedAsync(PickOrder pickOrder)
        {
            try
            {
                pickOrder.IsClosed = true;

                await repository.UpdateAsync(pickOrder, pickOrder.Category);

                return true;
            }
            catch
            {
                return false;
            }            
        }
    }
}
