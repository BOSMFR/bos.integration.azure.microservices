using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix
{
    public class ProductAttributeSearchResponseDTO : HttpResponse
    {
        public ICollection<ProductAttribute> Data { get; set; }
    }
}
