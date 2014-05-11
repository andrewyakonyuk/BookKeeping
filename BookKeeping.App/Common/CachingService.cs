using System;
using System.Diagnostics.Contracts;
using System.Runtime.Caching;

namespace BookKeeping.App.Common
{
    public class CacheService : ICacheService
    {
        private readonly MemoryCache _cache;
        static CacheService _this;
        static object _lock = new object();

        public static ICacheService Current
        {
            get
            {
                lock (_lock)
                {
                    if (_this == null)
                    {
                        _this = new CacheService();
                    }
                    return _this;
                }
            }
        }

        public CacheService()
        {
            this._cache = MemoryCache.Default;
        }

        public T Get<T>(string cacheKey)
        {
            Contract.Requires(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            return (T)this._cache.Get(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue)
        {
            Contract.Requires(string.IsNullOrEmpty(cacheKey), "cacheKey");
            Contract.Requires(cacheValue == null, "cacheValue");
            this._cache.Set(cacheKey, cacheValue, new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(5.0)
            });
        }

        public void Invalidate(string cacheKey)
        {
            Contract.Requires(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            this._cache.Remove(cacheKey);
        }
    }
}
