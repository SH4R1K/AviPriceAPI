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
                _memoryCache.Set("Storage", value);
            }
        }
        public List<CategoryTreePath>? CategoryTreePaths
        {
            get
            {
                return _memoryCache.Get("CategoryTreePath") as List<CategoryTreePath>;
            }
            set
            {
                _memoryCache.Set("CategoryTreePath", value);
            }
        }
        public List<LocationTreePath>? LocationTreePaths
        {
            get
            {
                return _memoryCache.Get("LocationTreePath") as List<LocationTreePath>;
            }
            set
            {
                _memoryCache.Set("LocationTreePath", value);
            }
        }
    }
}
