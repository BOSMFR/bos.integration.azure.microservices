using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class DeliveryPeriodService : IDeliveryPeriodService
    {
        private readonly IDeliveryPeriodRepository repository;
        private readonly IMapper mapper;

        public DeliveryPeriodService(IDeliveryPeriodRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<DeliveryPeriod> GetDeliveryPeriodAsync()
        {
            return (await repository.GetAllAsync(NavObjectCategory.DeliveryPeriod))?.FirstOrDefault();
        }

        public async Task<DeliveryPeriod> CreateOrUpdateDeliveryPeriodAsync(DeliveryPeriodDTO deliveryPeriodDTO)
        {
            var newDeliveryPeriod = this.mapper.Map<DeliveryPeriod>(deliveryPeriodDTO);

            var deliveryPeriod = (await repository.GetAllAsync(NavObjectCategory.DeliveryPeriod))?.FirstOrDefault();

            if (deliveryPeriod == null)
            {
                newDeliveryPeriod.Category = NavObjectCategory.DeliveryPeriod;
                newDeliveryPeriod.ReceivedFromErp = DateTime.UtcNow;

                await repository.AddAsync(newDeliveryPeriod, newDeliveryPeriod.Category);
            }
            else
            {
                newDeliveryPeriod.Id = deliveryPeriod.Id;
                newDeliveryPeriod.Category = deliveryPeriod.Category;
                newDeliveryPeriod.ReceivedFromErp = deliveryPeriod.ReceivedFromErp;

                await repository.UpdateAsync(newDeliveryPeriod, newDeliveryPeriod.Category);
            }

            return newDeliveryPeriod;
        }
    }
}
