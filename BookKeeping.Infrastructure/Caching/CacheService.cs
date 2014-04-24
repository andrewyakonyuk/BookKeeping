using System;
using System.Runtime.Caching;
using BookKeeping.Infrastructure.Dependency;
using System.Diagnostics.Contracts;
using Autofac;

namespace BookKeeping.Infrastructure.Caching
{
    public class CacheService : ICacheService
    {
        private readonly MemoryCache _cache;

        public static ICacheService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<ICacheService>();
            }
        }

        public CacheService()
        {
            this._cache = MemoryCache.Default;
        }

        public T GetCacheValue<T>(string cacheKey) where T : class
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            return (T)this._cache.Get(cacheKey);
        }

        public void SetCacheValue(string cacheKey, object cacheValue)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            Contract.Requires<ArgumentNullException>(cacheValue != null, "cacheValue");
            this._cache.Set(cacheKey, cacheValue, new CacheItemPolicy()
           {
               SlidingExpiration = TimeSpan.FromMinutes(10.0)
           });
        }

        public void Invalidate(string cacheKey)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            this._cache.Remove(cacheKey);
        }
    }
}
