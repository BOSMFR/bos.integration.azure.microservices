using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PackshotCreateUpdateDTO
    {
        public PlytixData<PlytixPackshotResponseData> PlytixResponseObject { get; set; }

        public PlytixPackshotUpdateCategoryDTO PackshotUpdateCategoryDTO { get; set; }
    }
}
