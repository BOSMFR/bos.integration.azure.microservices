using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;

namespace BOS.Integration.Azure.Microservices.Functions.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDTO, Product>();

            CreateMap<ProductDTO, PrimeCargoProductRequestDTO>();
        }
    }
}
