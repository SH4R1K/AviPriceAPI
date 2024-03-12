using AviAPI.Data;
using AviAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AviAPI.Classes
{
    public class StorageService
    {
        IMemoryCache _memoryCache;

        public StorageService(IMemoryCache memoryCache)
        { 
            _memoryCache = memoryCache;
        }

        public List<Matrix>? Matrices
        {
            get 
            {
                return _memoryCache.Get("Storage") as List<Matrix>;
            }
            set
            {
                _memoryCache.Set("Storage", value, DateTimeOffset.Now.AddYears(1));
            }
        }
    }
}
