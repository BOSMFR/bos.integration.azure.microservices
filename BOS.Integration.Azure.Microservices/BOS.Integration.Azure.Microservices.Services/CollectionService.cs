﻿using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Collection;
using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository repository;
        private readonly IMapper mapper;

        public CollectionService(ICollectionRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<CollectionEntity> GetCollectionAsync()
        {
            return (await repository.GetAllAsync(NavObjectCategory.Collection))?.FirstOrDefault();
        }

        public async Task<CollectionEntity> CreateOrUpdateCollectionAsync(CollectionDTO collectionDTO)
        {
            var newCollection = this.mapper.Map<CollectionEntity>(collectionDTO);

            var collection = (await repository.GetAllAsync(NavObjectCategory.Collection))?.FirstOrDefault();

            if (collection == null)
            {
                newCollection.Category = NavObjectCategory.Collection;
                newCollection.ReceivedFromErp = DateTime.UtcNow;

                await repository.AddAsync(newCollection, newCollection.Category);
            }
            else
            {
                newCollection.Id = collection.Id;
                newCollection.Category = collection.Category;
                newCollection.ReceivedFromErp = collection.ReceivedFromErp;

                await repository.UpdateAsync(newCollection, newCollection.Category);
            }

            return newCollection;
        }
    }
}
