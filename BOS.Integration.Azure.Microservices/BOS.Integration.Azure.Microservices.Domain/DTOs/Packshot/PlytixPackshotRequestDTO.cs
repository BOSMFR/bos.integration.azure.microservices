using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PlytixPackshotRequestDTO
    {
        public FileDTO File { get; set; }

        public string Brand { get; set; }

        public string CollectionCode { get; set; }

        public string DeliveryWindowCode { get; set; }

        public ImageType ImageType { get; set; }

        public ImageAngle ImageAngle { get; set; }
    }
}
