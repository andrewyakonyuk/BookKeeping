using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace BookKeeping.Infrastructure.Caching
{
    public static class CacheServiceExtensions
    {
        public static T Get<T>(this ICacheService cache, string cacheKey, Func<T> defaultValue)
        {
            Contract.Requires<ArgumentNullException>(defaultValue != null);
            var entity = cache.Get<T>(cacheKey);
            if (ReferenceEquals(entity, null))
            {
                entity = defaultValue();
                cache.Set(cacheKey, entity);
            }
            return entity;
        }
    }
}
