namespace BookKeeping.Infrastructure.Caching
{
    public interface ICacheService
    {
        T GetCacheValue<T>(string cacheKey) where T : class;

        void SetCacheValue(string cacheKey, object cacheValue);

        void Invalidate(string cacheKey);
    }
}
