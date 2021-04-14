using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PackshotService : IPackshotService
    {
        private readonly IPackshotRepository repository;
        private readonly IMapper mapper;

        public PackshotService(IPackshotRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<ActionExecutionResult> CreatePackshotAsync(PackshotDTO packshotDTO)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newPackshot = this.mapper.Map<Packshot>(packshotDTO);

                // Remove the first two characters from file name if the second character is '/'
                if (!string.IsNullOrEmpty(packshotDTO.File?.Name) && packshotDTO.File.Name[1] == '/')
                {
                    newPackshot.File.Name = packshotDTO.File.Name.Substring(2);
                }

                var packshot = await repository.GetByKeyParamsAsync(newPackshot, NavObjectCategory.Packshot);

                if (packshot == null)
                {
                    newPackshot.ReceivedFromSsis = DateTime.Now;

                    await repository.AddAsync(newPackshot, newPackshot.AssertType);

                    actionResult.Entity = newPackshot;
                    actionResult.Succeeded = true;
                }
                else
                {
                    actionResult.Entity = packshot;
                    actionResult.Error = $"The Packshot already exists";
                }

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<bool> UpdatePackshotFromPlytixInfoAsync(string packshotId, PlytixData<PlytixPackshotResponseData> plytixResponseObject)
        {
            var packshot = await repository.GetByIdAsync(packshotId, NavObjectCategory.Packshot);

            if (packshot == null)
            {
                return false;
            }

            packshot.PlytixPackshot = plytixResponseObject;

            await repository.UpdateAsync(packshot, packshot.AssertType);

            return true;
        }

        public async Task<List<Packshot>> GetPackshotsByFilterAsync(PackshotFilterDTO packshotFilter)
        {
            packshotFilter.FromDate ??= DateTime.MinValue;
            packshotFilter.ToDate = packshotFilter.ToDate.HasValue ? packshotFilter.ToDate.Value.AddDays(1) : DateTime.MaxValue;

            return await this.repository.GetByFilterAsync(packshotFilter, NavObjectCategory.Packshot);
        }
    }
}
