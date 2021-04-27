using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class AssetCategoryService : IAssetCategoryService
    {
        private readonly IAssetCategoryRepository repository;

        public AssetCategoryService(IAssetCategoryRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<string>> GetAssetCategoryIdsByPackshotInfoAsync(PlytixPackshotUpdateCategoryDTO plytixPackshot)
        {
            var assetCategoryIds = new List<string>();

            var assetCategories = await repository.GetAllAsync(plytixPackshot.Plytix.Id.ToString());

            // Find asset category by collection code
            var collectionPath = new List<string> { AssetCategoryName.Collection, !string.IsNullOrEmpty(plytixPackshot.CollectionCode) ? plytixPackshot.CollectionCode : AssetCategoryName.Default };

            string collectionCategoryId = assetCategories.Where(x => x.Path.SequenceEqual(collectionPath)).FirstOrDefault()?.Id;

            if (!string.IsNullOrEmpty(collectionCategoryId))
            {
                assetCategoryIds.Add(collectionCategoryId);
            }

            // Find asset category by delivery period code
            var deliveryPeriodPath = new List<string> { AssetCategoryName.DeliveryPeriod, !string.IsNullOrEmpty(plytixPackshot.DeliveryWindowCode) ? plytixPackshot.DeliveryWindowCode : AssetCategoryName.Default };

            string deliveryPeriodCategoryId = assetCategories.Where(x => x.Path.SequenceEqual(deliveryPeriodPath)).FirstOrDefault()?.Id;

            if (!string.IsNullOrEmpty(deliveryPeriodCategoryId))
            {
                assetCategoryIds.Add(deliveryPeriodCategoryId);
            }

            // Find asset category by image type id
            var imageTypePath = new List<string> { AssetCategoryName.ImageType, this.GetCategoryByImageType(plytixPackshot.ImageType?.Id) };

            string imageTypeCategoryId = assetCategories.Where(x => x.Path.SequenceEqual(imageTypePath)).FirstOrDefault()?.Id;

            if (!string.IsNullOrEmpty(imageTypeCategoryId))
            {
                assetCategoryIds.Add(imageTypeCategoryId);
            }

            // Find asset category by image angle name
            var imageAnglePath = new List<string> { AssetCategoryName.ImageAngle, !string.IsNullOrEmpty(plytixPackshot.ImageAngle.Name) ? plytixPackshot.ImageAngle.Name : AssetCategoryName.Default };

            string imageAngleCategoryId = assetCategories.Where(x => x.Path.SequenceEqual(imageAnglePath)).FirstOrDefault()?.Id;

            if (!string.IsNullOrEmpty(imageAngleCategoryId))
            {
                assetCategoryIds.Add(imageAngleCategoryId);
            }

            // Find asset category by brand
            var brandPath = new List<string> { AssetCategoryName.Brand, plytixPackshot.Plytix.Name};

            string brandCategoryId = assetCategories.Where(x => x.Path.SequenceEqual(brandPath)).FirstOrDefault()?.Id;

            if (!string.IsNullOrEmpty(brandCategoryId))
            {
                assetCategoryIds.Add(brandCategoryId);
            }

            return assetCategoryIds;
        }

        private string GetCategoryByImageType(string imageTypeId) => 
            imageTypeId switch
            {
                "1" => AssetCategoryName.ImageTypeSales,
                "2" => AssetCategoryName.ImageTypeShipment,
                _ => AssetCategoryName.Default
            };
    }
}
