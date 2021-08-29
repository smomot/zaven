using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Api.Repository
{
    public interface IOrderingRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetItemAsync(string id);

        Task CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
    }
}
