using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Utility.Cache
{
    public class MemCache
    {
        public static IMemoryCache _cache { get; set; }
        public MemCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public static void AddToCache(string cacheKey, object savedItem)
        {
            _cache.Set(cacheKey, savedItem);
        }

        public static T GetFromCache<T>(string cacheKey) where T : class
        {
            return _cache.Get<T>(cacheKey);
        }

        public static void RemoveFromCache(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }

        public static bool IsIncache(string cacheKey)
        {
            return _cache.TryGetValue(cacheKey, out _);
        }
    }
}
