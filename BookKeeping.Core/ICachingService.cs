
namespace BookKeeping.Core
{
    public interface ICacheService
    {
        T Get<T>(string cacheKey);

        void Set(string cacheKey, object cacheValue);

        void Invalidate(string cacheKey);
    }
}
