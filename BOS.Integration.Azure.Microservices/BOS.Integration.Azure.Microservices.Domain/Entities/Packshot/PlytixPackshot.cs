using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.Entities.Packshot
{
    public class PlytixData<T>
    {
        public ICollection<T> Data { get; set; }
    }
}
