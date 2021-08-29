using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Api.Infrastructure
{
    public interface IOrderingContext<T>
    {
        IMongoCollection<T> Items { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
