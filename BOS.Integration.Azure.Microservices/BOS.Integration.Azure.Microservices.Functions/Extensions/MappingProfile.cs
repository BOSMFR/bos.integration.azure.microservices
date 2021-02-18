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

            CreateMap<ProductDTO, PrimeCargoProductRequestDTO>()
                .ForMember(x => x.Barcode, x => x.MapFrom(x => x.EanNo))
                .ForMember(x => x.PartNumber, x => x.MapFrom(x => x.ItemNo))
                .ForMember(x => x.Variant1, x => x.MapFrom(x => (x.Colour.Code + " " + x.Colour.Description).Trim()))
                .ForMember(x => x.Variant2, x => x.MapFrom(x => x.Size.Code))
                .ForMember(x => x.Variant3, x => x.MapFrom(x => x.Style.Code))
                .ForMember(x => x.Variant4, x => x.MapFrom(x => (x.Assortment.Code + " " + x.Assortment.Description).Trim()))
                .ForMember(x => x.Variant5, x => x.MapFrom(x => x.Sku));
        }
    }
}
