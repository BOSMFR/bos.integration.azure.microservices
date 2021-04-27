using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PlytixPackshotUpdateCategoryDTO
    {
        public string AssetId { get; set; }

        public PlytixInstance Plytix { get; set; }

        public string PlytixToken { get; set; }

        public string Brand { get; set; }

        public string CollectionCode { get; set; }

        public string DeliveryWindowCode { get; set; }

        public ImageType ImageType { get; set; }

        public ImageAngle ImageAngle { get; set; }
    }
}
