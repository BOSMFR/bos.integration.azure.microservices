using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Collection;
using BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Noos;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;
using BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Entities.Noos;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.Entities.Shopify;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using DeliveryPeriodEntity = BOS.Integration.Azure.Microservices.Domain.Entities.DeliveryPeriod.DeliveryPeriod;
using GoodsReceivalEntity = BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival.GoodsReceival;
using NoosEntity = BOS.Integration.Azure.Microservices.Domain.Entities.Noos.Noos;
using PackshotEntity = BOS.Integration.Azure.Microservices.Domain.Entities.Packshot.Packshot;
using PickOrderEntity = BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder.PickOrder;
using ProductEntity = BOS.Integration.Azure.Microservices.Domain.Entities.Product.Product;

namespace BOS.Integration.Azure.Microservices.Functions.Extensions
{
    public class MappingProfile : Profile
    {
        private const int VariantMaxLength = 20;

        public MappingProfile()
        {
            CreateMap<ProductDTO, ProductEntity>();
            CreateMap<ProductEntity, ProductDTO>();

            CreateMap<CollectionDTO, CollectionEntity>()
                .ForMember(x => x.Details, x => x.MapFrom(x => x.Season));
            CreateMap<CollectionDetailsDTO, CollectionDetails>()
                .ForMember(x => x.Id, x => x.MapFrom(x => x.Code));

            CreateMap<DeliveryPeriodDTO, DeliveryPeriodEntity>()
                .ForMember(x => x.Details, x => x.MapFrom(x => x.DeliveryWindow));
            CreateMap<DeliveryPeriodDetailsDTO, DeliveryPeriodDetails>()
                .ForMember(x => x.Id, x => x.MapFrom(x => x.Code));

            CreateMap<NoosDTO, NoosEntity>()
                .ForMember(x => x.StyleNo, x => x.MapFrom(x => x.ItemNo))
                .ForMember(x => x.StyleDescription, x => x.MapFrom(x => x.ItemDescription));
            CreateMap<NoosDetailsDTO, NoosDetails>()
                .ForMember(x => x.ColorId, x => x.MapFrom(x => x.ColorCode));

            CreateMap<ProductEntity, ProductGridDTO>()
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId));

            CreateMap<ProductDTO, PrimeCargoProductRequestDTO>()
                .ForMember(x => x.Barcode, x => x.MapFrom(x => x.EanNo))
                .ForMember(x => x.PartNumber, x => x.MapFrom(x => x.ItemNo))
                .ForMember(x => x.TypeId, x => x.MapFrom(x => MapPrimeCargoProductType(x.WmsProductType)))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId))
                .ForMember(x => x.Variant1, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Colour.Code + " " + x.Colour.Description)))
                .ForMember(x => x.Variant2, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Size.Code)))
                .ForMember(x => x.Variant3, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Style.Code)))
                .ForMember(x => x.Variant4, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Assortment.Code + " " + x.Assortment.Description)))
                .ForMember(x => x.Variant5, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Sku)));

            CreateMap<ProductEntity, PrimeCargoProductRequestDTO>()
                .ForMember(x => x.Barcode, x => x.MapFrom(x => x.EanNo))
                .ForMember(x => x.PartNumber, x => x.MapFrom(x => x.ItemNo))
                .ForMember(x => x.TypeId, x => x.MapFrom(x => MapPrimeCargoProductType(x.WmsProductType)))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId))
                .ForMember(x => x.Variant1, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Colour.Code + " " + x.Colour.Description)))
                .ForMember(x => x.Variant2, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Size.Code)))
                .ForMember(x => x.Variant3, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Style.Code)))
                .ForMember(x => x.Variant4, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Assortment.Code + " " + x.Assortment.Description)))
                .ForMember(x => x.Variant5, x => x.MapFrom(x => MapPrimeCargoProductVariant(x.Sku)));

            CreateMap<PrimeCargoResponseContent<PrimeCargoProductResponseData>, PrimeCargoProductResponseDTO>()
                .ForMember(x => x.EnaNo, x => x.MapFrom(x => x.Data.Barcode))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.Data.ProductId))
                .ForMember(x => x.Sku, x => x.MapFrom(x => x.Data.Variant5))
                .ForMember(x => x.ResponseDateTime, x => x.MapFrom(x => x.Data.CreatedTime))
                .ForMember(x => x.ResponseCode, x => x.MapFrom(x => x.StatusCode));

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

            CreateMap<Shop, ShopDTO>()
                .ForMember(x => x.ApiUrl, x => x.MapFrom(x => x.Api.ServerUrl));

            CreateMap<GoodsReceivalDTO, GoodsReceivalEntity>();
            CreateMap<GoodsReceivalEntity, GoodsReceivalDTO>();

            CreateMap<GoodsReceivalDTO, PrimeCargoGoodsReceivalRequestDTO>()
                .ForMember(x => x.ReceivalNumber, x => x.MapFrom(x => x.WmsDocumentNo))
                .ForMember(x => x.Lines, x => x.MapFrom(x => x.PurchaseLines));

            CreateMap<PurchaseLine, PrimeCargoPurchaseLineRequestDTO>()
                .ForMember(x => x.ExtReference, x => x.MapFrom(x => x.WmsDocumentLineNo))
                .ForMember(x => x.Qty, x => x.MapFrom(x => x.QtyToReceive))
                .ForMember(x => x.ProductId, x => x.MapFrom(x => x.PrimeCargoProductId));

            CreateMap<PickOrderDTO, PickOrderEntity>();
            CreateMap<PickOrderEntity, PickOrderDTO>();

            CreateMap<PickOrderDTO, PrimeCargoPickOrderRequestDTO>()
                .ForMember(x => x.CountryId, x => x.MapFrom(x => int.Parse(x.CountryIsoCode)))
                .ForMember(x => x.ShippingProductId, x => x.MapFrom(x => int.Parse(x.ShippingProductId)))
                .ForMember(x => x.SubOwnerId, x => x.MapFrom(x => int.Parse(x.SubOwnerId)))
                .ForMember(x => x.SubOwnerAddressId, x => x.MapFrom(x => int.Parse(x.SubOwnerAddressId)))
                .ForMember(x => x.UsStateId, x => x.MapFrom(x => int.Parse(x.UsStateId)))
                .ForMember(x => x.CustomerId1, x => x.MapFrom(x => x.CustomerID1))
                .ForMember(x => x.CustomerId2, x => x.MapFrom(x => x.CustomerID2))
                .ForMember(x => x.CustomerId3, x => x.MapFrom(x => x.CustomerID3))
                .ForMember(x => x.Lcid, x => x.MapFrom(x => x.LcId))
                .ForMember(x => x.Lines, x => x.MapFrom(x => x.SalesLines));

            CreateMap<SalesLine, PrimeCargoSalesLineDTO>()
                .ForMember(x => x.ProductId, x => x.MapFrom(x => int.Parse(x.ProductId)))
                .ForMember(x => x.Properties, x => x.MapFrom(x => x.Proporties));

            CreateMap<SalesLineProperty<string>, SalesLineProperty<long>>()
                .ForMember(x => x.TariffNumber, x => x.MapFrom(x => long.Parse(x.TariffNumber)));

            CreateMap<PackshotDTO, PackshotEntity>();
            CreateMap<PackshotEntity, PackshotDTO>();
            CreateMap<PackshotEntity, PlytixPackshotRequestDTO>()
                .ForMember(x => x.Brand, x => x.MapFrom(x => x.Product.Brand))
                .ForMember(x => x.CollectionCode, x => x.MapFrom(x => x.Product.CollectionCode))
                .ForMember(x => x.DeliveryWindowCode, x => x.MapFrom(x => x.Product.DeliveryWindowCode));

            CreateMap<PlytixPackshotRequestDTO, PlytixPackshotUpdateCategoryDTO>();

            CreateMap<File, FileDTO>();

            CreateMap<GoodsReceivalClosedDTO, GoodsReceivalClosed>();
            CreateMap<GoodsReceivalClosed, GoodsReceivalClosedDTO>();

            CreateMap<GoodsReceivalLineCreatedDTO, GoodsReceivalLineCreated>();
            CreateMap<GoodsReceivalLineCreated, GoodsReceivalLineCreatedDTO>();
        }

        private PrimeCargoProductType? MapPrimeCargoProductType(string wmsProductType) => 
            wmsProductType switch
            {
                "F" => PrimeCargoProductType.F,
                "B" => PrimeCargoProductType.B,
                _ => null,
            };

        private string MapPrimeCargoProductVariant(string variant) => variant.Length > VariantMaxLength ?  variant.Substring(0, VariantMaxLength).Trim() : variant;
    }
}
