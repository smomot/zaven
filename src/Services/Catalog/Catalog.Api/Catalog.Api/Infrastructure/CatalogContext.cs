using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Infrastructure
{
    public class CatalogContext<T> : ICatalogContext<T>
    {
        private IMongoDatabase _db { get; set; }
        public CatalogContext(IDatabaseSettings configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            _db = client.GetDatabase(configuration.DatabaseName);
            Items = GetCollection<T>(configuration.CollectionName);
        }

        public IMongoCollection<T> Items { get; }


        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}
