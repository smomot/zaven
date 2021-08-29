using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Infrastructure
{
    public interface ICatalogContext<T>
    {
        IMongoCollection<T> Items { get; }
        IMongoCollection<T> GetCollection<T>(string name);

    }
}
