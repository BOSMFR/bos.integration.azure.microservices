using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;

namespace BOS.Integration.Azure.Microservices.Functions.External.Extensions
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GoodsReceivalClosedDTO, GoodsReceivalClosed>();
            CreateMap<GoodsReceivalClosed, GoodsReceivalClosedDTO>();

            CreateMap<GoodsReceivalLineCreatedDTO, GoodsReceivalLineCreated>();
            CreateMap<GoodsReceivalLineCreated, GoodsReceivalLineCreatedDTO>();
        }
    }
}
