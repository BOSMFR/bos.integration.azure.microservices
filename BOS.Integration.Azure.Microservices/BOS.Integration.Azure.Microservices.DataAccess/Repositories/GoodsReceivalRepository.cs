using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class GoodsReceivalRepository : CosmosDbRepository<GoodsReceival>, IGoodsReceivalRepository
    {
        public override string ContainerName { get; } = "product";

        public override string GenerateId(GoodsReceival entity) => entity.WmsDocumentNo;

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public GoodsReceivalRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        public async Task<List<GoodsReceival>> GetByFilterAsync(GoodsReceivalFilterDTO goodsReceivalFilter, string partitionKey = null)
        {
            QueryRequestOptions requestOptions = null;

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions = new QueryRequestOptions
                {
                    PartitionKey = this.ResolvePartitionKey(partitionKey)
                };
            }

            Expression<Func<GoodsReceival, bool>> query = g => (g.ReceivedFromErp >= goodsReceivalFilter.FromDate && g.ReceivedFromErp < goodsReceivalFilter.ToDate)
                                                        && (!goodsReceivalFilter.PrimeCargoGoodsReceivalId.HasValue || (g.PrimeCargoData != null && g.PrimeCargoData.GoodsReceivalId == goodsReceivalFilter.PrimeCargoGoodsReceivalId))
                                                        && (string.IsNullOrEmpty(goodsReceivalFilter.WmsDocumentNo) || g.WmsDocumentNo.Contains(goodsReceivalFilter.WmsDocumentNo));

            var iterator = _container.GetItemLinqQueryable<GoodsReceival>(requestOptions: requestOptions).Where(query).ToFeedIterator();

            return (await iterator.ReadNextAsync()).ToList();
        }
    }
}
