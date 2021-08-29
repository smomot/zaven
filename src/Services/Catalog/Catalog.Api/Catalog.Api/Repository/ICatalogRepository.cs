using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Repository
{
    public interface ICatalogRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetItemAsync(string id);

        Task CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
    }
}
