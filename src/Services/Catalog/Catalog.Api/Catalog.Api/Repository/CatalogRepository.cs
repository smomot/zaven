using Catalog.Api.Infrastructure;
using Catalog.Api.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Repository
{
    public class CatalogRepository<T> : ICatalogRepository<T>
    {
        private readonly ICatalogContext<T> _context;

        private readonly IMongoCollection<T> _dbCollection;

        public CatalogRepository(ICatalogContext<T> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbCollection = _context.GetCollection<T>(typeof(T).Name);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbCollection
                           .Find(p => true)
                           .ToListAsync();
        }

        public async Task<T> GetItemAsync(string id)
        {
            var filter = new BsonDocument { { "Id", id } };

 
            return await _dbCollection
                           .Find(filter)
                           .FirstOrDefaultAsync();
        }

        public async Task CreateItemAsync(T item)
        {
            await _dbCollection.InsertOneAsync(item);
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            var generiItem = item as BaseEntity;
            if (generiItem != null)
            {
                var filter = new BsonDocument { { "Id", generiItem.Id } };
                var updateResult = await _dbCollection
                                            .ReplaceOneAsync(filter, replacement: item);

                return updateResult.IsAcknowledged
                        && updateResult.ModifiedCount > 0;
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var filter = new BsonDocument { { "Id", id } };
            //FilterDefinition<T> filter = Builders<T>.Filter.Eq(p => p.Id, id);


            DeleteResult deleteResult = await _dbCollection
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }





    }
}
