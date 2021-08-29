using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ordering.Api.Infrastructure
{
    public class OrderingContext<T> : IOrderingContext<T>
    {
        private IMongoDatabase _db { get; set; }
        public OrderingContext(IDatabaseSettings configuration)
        {


            DisplayNameAttribute collectionType = this.GetType().GenericTypeArguments[0].GetCustomAttributes().First() as DisplayNameAttribute;
            string collectionName = collectionType.DisplayName; 
            var client = new MongoClient(configuration.ConnectionString);
            _db = client.GetDatabase(configuration.DatabaseName);
            Items = GetCollection<T>(collectionName);
        }

        public IMongoCollection<T> Items { get; }


        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}
