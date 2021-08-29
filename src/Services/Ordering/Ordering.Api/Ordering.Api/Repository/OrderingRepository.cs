using MongoDB.Bson;
using MongoDB.Driver;
using Ordering.Api.Infrastructure;
using Ordering.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Api.Repository
{
    public class OrderingRepository<T> : IOrderingRepository<T>
    {
        private readonly IOrderingContext<T> _context;

        private readonly IMongoCollection<T> _dbCollection;

        public OrderingRepository(IOrderingContext<T> context)
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
