using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Packshot
{
    public class Packshot : AssertEntity
    {
        public string Source { get; set; }

        public File File { get; set; }

        public Product Product { get; set; }

        public ImageType ImageType { get; set; }

        public ImageAngle ImageAngle { get; set; }

        public ImageFormat Imageformat { get; set; }

        public Tracking Tracking { get; set; }

        public PlytixData<PlytixPackshotResponseData> PlytixPackshot { get; set; }
    }
}
