using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Noos;
using BOS.Integration.Azure.Microservices.Domain.Entities.Noos;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class NoosService : INoosService
    {
        private readonly INoosRepository repository;
        private readonly IMapper mapper;

        public NoosService(INoosRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Noos> CreateOrUpdateNoosAsync(NoosDTO noosDTO)
        {
            var newNoos = this.mapper.Map<Noos>(noosDTO);

            var noos = await repository.GetByIdAsync(newNoos.StyleNo, NavObjectCategory.Noos);

            if (noos == null)
            {
                newNoos.Category = NavObjectCategory.Noos;
                newNoos.ReceivedFromErp = DateTime.UtcNow;

                await repository.AddAsync(newNoos, newNoos.Category);
            }
            else
            {
                newNoos.Id = noos.Id;
                newNoos.Category = noos.Category;
                newNoos.ReceivedFromErp = noos.ReceivedFromErp;

                await repository.UpdateAsync(newNoos, newNoos.Category);
            }

            return newNoos;
        }
    }
}
