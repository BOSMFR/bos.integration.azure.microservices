using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository repository;
        private readonly IMapper mapper;

        public ShopService(IShopRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<ShopDTO>> GetAllActiveAsync(string partitionKey = null)
        {
            var shops = await this.repository.GetAllActiveAsync(partitionKey);

            return this.mapper.Map<List<ShopDTO>>(shops);
        }
    }
}
