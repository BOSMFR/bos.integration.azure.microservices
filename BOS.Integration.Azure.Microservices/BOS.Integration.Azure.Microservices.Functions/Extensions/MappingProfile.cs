using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Shopify;
using BOS.Integration.Azure.Microservices.Domain.Enums;

namespace BOS.Integration.Azure.Microservices.Functions.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDTO, Product>();
            CreateMap<Product, ProductDTO>();

            CreateMap<Product, ProductGridDTO>()
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId));

            CreateMap<ProductDTO, PrimeCargoProductRequestDTO>()
                .ForMember(x => x.Barcode, x => x.MapFrom(x => x.EanNo))
                .ForMember(x => x.PartNumber, x => x.MapFrom(x => x.ItemNo))
                .ForMember(x => x.TypeId, x => x.MapFrom(x => MapPrimeCargoProductType(x.WmsProductType)))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId))
                .ForMember(x => x.Variant1, x => x.MapFrom(x => (x.Colour.Code + " " + x.Colour.Description).Trim()))
                .ForMember(x => x.Variant2, x => x.MapFrom(x => x.Size.Code))
                .ForMember(x => x.Variant3, x => x.MapFrom(x => x.Style.Code))
                .ForMember(x => x.Variant4, x => x.MapFrom(x => (x.Assortment.Code + " " + x.Assortment.Description).Trim()))
                .ForMember(x => x.Variant5, x => x.MapFrom(x => x.Sku));

            CreateMap<Product, PrimeCargoProductRequestDTO>()
                .ForMember(x => x.Barcode, x => x.MapFrom(x => x.EanNo))
                .ForMember(x => x.PartNumber, x => x.MapFrom(x => x.ItemNo))
                .ForMember(x => x.TypeId, x => x.MapFrom(x => MapPrimeCargoProductType(x.WmsProductType)))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId))
                .ForMember(x => x.Variant1, x => x.MapFrom(x => (x.Colour.Code + " " + x.Colour.Description).Trim()))
                .ForMember(x => x.Variant2, x => x.MapFrom(x => x.Size.Code))
                .ForMember(x => x.Variant3, x => x.MapFrom(x => x.Style.Code))
                .ForMember(x => x.Variant4, x => x.MapFrom(x => (x.Assortment.Code + " " + x.Assortment.Description).Trim()))
                .ForMember(x => x.Variant5, x => x.MapFrom(x => x.Sku));

            CreateMap<PrimeCargoProductResponseContent, PrimeCargoProductResponseDTO>()
                .ForMember(x => x.EnaNo, x => x.MapFrom(x => x.Data.Barcode))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.Data.ProductId))
                .ForMember(x => x.Sku, x => x.MapFrom(x => x.Data.Variant5))
                .ForMember(x => x.ResponseDateTime, x => x.MapFrom(x => x.Data.CreatedTime))
                .ForMember(x => x.ResponseCode, x => x.MapFrom(x => x.StatusCode));

            CreateMap<Product, LogInfo>()
                .ForMember(x => x.ObjectId, x => x.MapFrom(x => x.Id))
                .ForMember(x => x.Object, x => x.MapFrom(x => x.Category));
            CreateMap<LogInfo, ErpMessage>();
            CreateMap<LogInfo, TimeLine>();

            CreateMap<Shop, ShopDTO>()
                .ForMember(x => x.ApiUrl, x => x.MapFrom(x => x.Api.ServerUrl));
        }

        private PrimeCargoProductType? MapPrimeCargoProductType(string wmsProductType) => 
            wmsProductType switch
            {
                "F" => PrimeCargoProductType.F,
                "B" => PrimeCargoProductType.B,
                _ => null,
            };
    }
}
