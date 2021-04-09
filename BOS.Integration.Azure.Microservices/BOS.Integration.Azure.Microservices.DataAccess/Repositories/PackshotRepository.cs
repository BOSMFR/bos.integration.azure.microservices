using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class PackshotRepository : CosmosDbRepository<Packshot>, IPackshotRepository
    {
        public override string ContainerName { get; } = "assert";

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
    }
}
