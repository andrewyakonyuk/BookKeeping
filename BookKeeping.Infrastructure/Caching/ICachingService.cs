using System;
using System.Diagnostics.Contracts;
namespace BookKeeping.Infrastructure.Caching
{
    [ContractClass(typeof(CacheServiceContract))]
    public interface ICacheService
    {
        T GetValue<T>(string cacheKey);

        void SetValue(string cacheKey, object cacheValue);

        void Invalidate(string cacheKey);
    }

    [ContractClassFor(typeof(ICacheService))]
    internal abstract class CacheServiceContract : ICacheService
    {
        T ICacheService.GetValue<T>(string cacheKey)
        {
            Contract.Requires(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            throw new NotImplementedException();
        }

        void ICacheService.SetValue(string cacheKey, object cacheValue)
        {
            Contract.Requires(!string.IsNullOrEmpty(cacheKey), "cacheKey");
            Contract.Requires(!ReferenceEquals(cacheValue, null), "cacheValue");
        }

        void ICacheService.Invalidate(string cacheKey)
        {
            Contract.Requires(!string.IsNullOrEmpty(cacheKey), "cacheKey");
        }
    }
}
