using System;
using System.Collections.Generic;

namespace BookKeeping.App.Infrastructure.Caching
{
    public static class CacheServiceExtensions
    {
        public static T Get<T>(this ICacheService cache, string cacheKey, Func<T> defaultValue)
        {
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
