using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class ShippingMethodService : IShippingMethodService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "ShippingMethods";
        private readonly IShippingMethodRepository _repository;
        private readonly ICacheService _cacheService;

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

        public IEnumerable<BookKeeping.Domain.Models.ShippingMethod> GetAll(long storeId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)Enumerable.OrderBy<BookKeeping.Domain.Models.ShippingMethod, int>(Enumerable.Where<BookKeeping.Domain.Models.ShippingMethod>((IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.ShippingMethod, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.ShippingMethod, int>)(i => i.Sort));
        }

        public IEnumerable<BookKeeping.Domain.Models.ShippingMethod> GetAllAllowedIn(long storeId, long countryId, long? countryRegionId = null)
        {
            return (IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)Enumerable.ToList<BookKeeping.Domain.Models.ShippingMethod>(Enumerable.Where<BookKeeping.Domain.Models.ShippingMethod>(this.GetAll(storeId), (Func<BookKeeping.Domain.Models.ShippingMethod, bool>)(sm =>
            {
                if (!sm.AllowedInFollowingCountries.Contains(countryId))
                    return false;
                if (countryRegionId.HasValue)
                    return sm.AllowedInFollowingCountryRegions.Contains(countryRegionId.Value);
                else
                    return true;
            })));
        }

        public BookKeeping.Domain.Models.ShippingMethod Get(long storeId, long shippingMethodId)
        {
            List<BookKeeping.Domain.Models.ShippingMethod> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.ShippingMethod shippingMethod = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.ShippingMethod>((IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)cachedList, (Func<BookKeeping.Domain.Models.ShippingMethod, bool>)(i => i.Id == shippingMethodId));
            if (shippingMethod == null)
            {
                lock (this._cacheLock)
                {
                    shippingMethod = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.ShippingMethod>((IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)cachedList, (Func<BookKeeping.Domain.Models.ShippingMethod, bool>)(i => i.Id == shippingMethodId));
                    if (shippingMethod == null)
                    {
                        shippingMethod = this._repository.Get(storeId, shippingMethodId);
                        if (shippingMethod != null)
                            cachedList.Add(shippingMethod);
                    }
                }
            }
            return shippingMethod;
        }

        private void ShippingMethodCreated(BookKeeping.Domain.Models.ShippingMethod shippingMethod)
        {
            List<BookKeeping.Domain.Models.ShippingMethod> cachedList = this.GetCachedList(shippingMethod.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.ShippingMethod>((IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)cachedList, (Func<BookKeeping.Domain.Models.ShippingMethod, bool>)(sm => sm.Id == shippingMethod.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.ShippingMethod>((IEnumerable<BookKeeping.Domain.Models.ShippingMethod>)cachedList, (Func<BookKeeping.Domain.Models.ShippingMethod, bool>)(sm => sm.Id != shippingMethod.Id)))
                    return;
                cachedList.Add(shippingMethod);
            }
        }

        private List<BookKeeping.Domain.Models.ShippingMethod> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.ShippingMethod> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.ShippingMethod>>("ShippingMethods-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.ShippingMethod>>("ShippingMethods-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.ShippingMethod>(this._repository.GetAll(storeId));
                        this._cacheService.SetCacheValue("ShippingMethods-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}