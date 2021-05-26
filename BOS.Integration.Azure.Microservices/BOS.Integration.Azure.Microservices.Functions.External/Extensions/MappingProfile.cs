using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;

namespace BOS.Integration.Azure.Microservices.Functions.External.Extensions
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WebhookInfoDTO, WebhookInfo>();

            CreateMap<GoodsReceivalLineCreatedDTO, GoodsReceivalLineCreated>();
            CreateMap<GoodsReceivalLineCreated, GoodsReceivalLineCreatedDTO>();

            CreateMap<ErpEntity, LogInfo>()
                .ForMember(x => x.ObjectId, x => x.MapFrom(x => x.Id))
                .ForMember(x => x.Object, x => x.MapFrom(x => x.Category));

            CreateMap<AssetEntity, LogInfo>()
                .ForMember(x => x.ObjectId, x => x.MapFrom(x => x.Id))
                .ForMember(x => x.Object, x => x.MapFrom(x => x.AssetType))
                .ForMember(x => x.ErpDateTime, x => x.MapFrom(x => x.Created))
                .ForMember(x => x.ReceivedFromErp, x => x.MapFrom(x => x.ReceivedFromSsis));

            CreateMap<LogInfo, ErpMessage>();
            CreateMap<LogInfo, TimeLine>();
        }
    }
}
