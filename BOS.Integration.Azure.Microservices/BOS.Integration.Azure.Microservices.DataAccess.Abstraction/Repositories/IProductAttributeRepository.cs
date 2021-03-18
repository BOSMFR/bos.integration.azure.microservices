﻿using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IProductAttributeRepository : IRepository<ProductAttribute>
    {
        Task<ProductAttribute> GetByLabelAsync(string label, string partitionKey = null);
    }
}
