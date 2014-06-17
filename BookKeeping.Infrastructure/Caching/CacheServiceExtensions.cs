using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace BookKeeping.Caching
{
    public static class CacheServiceExtensions
    {
        public static T Get<T>(this ICacheService cache, string cacheKey, Func<T> defaultValue)
        {
            var entity = cache.GetValue<T>(cacheKey);
            if (ReferenceEquals(entity, null))
            {
                entity = defaultValue();
                cache.SetValue(cacheKey, entity);
            }
            return entity;
        }
    }
}
