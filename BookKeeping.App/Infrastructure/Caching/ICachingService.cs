using System;
namespace BookKeeping.App.Infrastructure.Caching
{
    public interface ICacheService
    {
        T Get<T>(string cacheKey);

        void Set(string cacheKey, object cacheValue);

        void Invalidate(string cacheKey);
    }
}
