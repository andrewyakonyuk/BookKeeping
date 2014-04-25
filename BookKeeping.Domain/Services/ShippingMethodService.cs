using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class ShippingMethodService : IShippingMethodService
    {
        private const string CacheKey = "ShippingMethods";
        private readonly IShippingMethodRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

        public static IShippingMethodService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IShippingMethodService>();
            }
        }

        public ShippingMethodService(IShippingMethodRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.ShippingMethod.Created += new ShippingMethodEventHandler(this.ShippingMethodCreated);
        }

        public IEnumerable<ShippingMethod> GetAll(long storeId)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public IEnumerable<ShippingMethod> GetAllAllowedIn(long storeId, long countryId, long? countryRegionId = null)
        {
            return (
                from sm in this.GetAll(storeId)
                where sm.AllowedInFollowingCountries.Contains(countryId) && (!countryRegionId.HasValue || sm.AllowedInFollowingCountryRegions.Contains(countryRegionId.Value))
                select sm).ToList<ShippingMethod>();
        }

        public ShippingMethod Get(long storeId, long shippingMethodId)
        {
            List<ShippingMethod> cachedList = this.GetCachedList(storeId);
            ShippingMethod shippingMethod = cachedList.SingleOrDefault((ShippingMethod i) => i.Id == shippingMethodId);
            if (shippingMethod == null)
            {
                lock (this._cacheLock)
                {
                    shippingMethod = cachedList.SingleOrDefault((ShippingMethod i) => i.Id == shippingMethodId);
                    if (shippingMethod == null)
                    {
                        shippingMethod = this._repository.Get(storeId, shippingMethodId);
                        if (shippingMethod != null)
                        {
                            cachedList.Add(shippingMethod);
                        }
                    }
                }
            }
            return shippingMethod;
        }

        private void ShippingMethodCreated(ShippingMethod shippingMethod)
        {
            List<ShippingMethod> cachedList = this.GetCachedList(shippingMethod.StoreId);
            if (cachedList.Any((ShippingMethod sm) => sm.Id == shippingMethod.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((ShippingMethod sm) => sm.Id != shippingMethod.Id))
                {
                    cachedList.Add(shippingMethod);
                }
            }
        }

        private List<ShippingMethod> GetCachedList(long storeId)
        {
            List<ShippingMethod> list = this._cacheService.GetCacheValue<List<ShippingMethod>>("ShippingMethods-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<ShippingMethod>>("ShippingMethods-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId).ToList<ShippingMethod>();
                        this._cacheService.SetCacheValue("ShippingMethods-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}