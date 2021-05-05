using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;

using PackshotProduct = BOS.Integration.Azure.Microservices.Domain.Entities.Packshot.Product;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PackshotDTO
    {
        public string AssetType { get; set; }

        public string Source { get; set; }

        public string Id { get; set; }

        public File File { get; set; }

        public PackshotProduct Product { get; set; }

        public ImageType ImageType { get; set; }

        public ImageAngle ImageAngle { get; set; }

        public ImageFormat Imageformat { get; set; }

        public Tracking Tracking { get; set; }

        public string Created { get; set; }
    }
}
