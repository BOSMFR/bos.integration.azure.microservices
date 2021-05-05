using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class PackshotRepository : CosmosDbRepository<Packshot>, IPackshotRepository
    {
        public override string ContainerName { get; } = "asset";

        public override string GenerateId(Packshot entity) => entity.Id;

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public PackshotRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<Packshot> GetByKeyParamsAsync(Packshot packshot, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            Expression<Func<Packshot, bool>> filterQuery = x => x.Id == packshot.Id
                                                            || (x.Product.StyleNo == packshot.Product.StyleNo 
                                                                && x.Product.Colorid == packshot.Product.Colorid
                                                                && x.ImageType.Id == packshot.ImageType.Id
                                                                && x.ImageAngle.Id == packshot.ImageAngle.Id
                                                                && x.Imageformat.Id == packshot.Imageformat.Id);

            var iterator = _container.GetItemLinqQueryable<Packshot>(requestOptions: requestOptions).Where(filterQuery).ToFeedIterator();

            return (await iterator.ReadNextAsync()).FirstOrDefault();
        }

        public async Task<List<Packshot>> GetByFilterAsync(PackshotFilterDTO packshotFilter, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            Expression<Func<Packshot, bool>> query = p => (p.ReceivedFromSsis >= packshotFilter.FromDate && p.ReceivedFromSsis < packshotFilter.ToDate)
                                                        && (string.IsNullOrEmpty(packshotFilter.StyleNo) || p.Product.StyleNo == packshotFilter.StyleNo)
                                                        && (string.IsNullOrEmpty(packshotFilter.StyleName) || p.Product.StyleDescription.Contains(packshotFilter.StyleName))
                                                        && (string.IsNullOrEmpty(packshotFilter.ColorNo) || p.Product.Colorid == packshotFilter.ColorNo)
                                                        && (string.IsNullOrEmpty(packshotFilter.ColorName) || p.Product.ColorName.Contains(packshotFilter.ColorName))
                                                        && (string.IsNullOrEmpty(packshotFilter.Collection) || p.Product.CollectionCode == packshotFilter.Collection);

            var iterator = _container.GetItemLinqQueryable<Packshot>(requestOptions: requestOptions).Where(query).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
