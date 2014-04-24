using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class CountryRegionService : ICountryRegionService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "CountryRegions";
        private readonly ICountryRegionRepository _repository;
        private readonly ICacheService _cacheService;

        public static ICountryRegionService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<ICountryRegionService>();
            }
        }

        public CountryRegionService(ICountryRegionRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.CountryRegion.Created += new CountryRegionEventHandler(this.CountryRegionCreated);
        }

        public IEnumerable<BookKeeping.Domain.Models.CountryRegion> GetAll(long storeId, long? countryId = null)
        {
            return (IEnumerable<BookKeeping.Domain.Models.CountryRegion>)Enumerable.OrderBy<BookKeeping.Domain.Models.CountryRegion, int>(Enumerable.Where<BookKeeping.Domain.Models.CountryRegion>((IEnumerable<BookKeeping.Domain.Models.CountryRegion>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.CountryRegion, bool>)(i =>
            {
                if (i.IsDeleted)
                    return false;
                if (!countryId.HasValue)
                    return true;
                long local_0 = i.CountryId;
                long? local_1 = countryId;
                if (local_0 == local_1.GetValueOrDefault())
                    return local_1.HasValue;
                else
                    return false;
            })), (Func<BookKeeping.Domain.Models.CountryRegion, int>)(i => i.Sort));
        }

        public BookKeeping.Domain.Models.CountryRegion Get(long storeId, long countryRegionId)
        {
            List<BookKeeping.Domain.Models.CountryRegion> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.CountryRegion countryRegion = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.CountryRegion>((IEnumerable<BookKeeping.Domain.Models.CountryRegion>)cachedList, (Func<BookKeeping.Domain.Models.CountryRegion, bool>)(i => i.Id == countryRegionId));
            if (countryRegion == null)
            {
                lock (this._cacheLock)
                {
                    countryRegion = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.CountryRegion>((IEnumerable<BookKeeping.Domain.Models.CountryRegion>)cachedList, (Func<BookKeeping.Domain.Models.CountryRegion, bool>)(i => i.Id == countryRegionId));
                    if (countryRegion == null)
                    {
                        countryRegion = this._repository.Get(storeId, countryRegionId);
                        if (countryRegion != null)
                            cachedList.Add(countryRegion);
                    }
                }
            }
            return countryRegion;
        }

        private void CountryRegionCreated(BookKeeping.Domain.Models.CountryRegion countryRegion)
        {
            List<BookKeeping.Domain.Models.CountryRegion> cachedList = this.GetCachedList(countryRegion.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.CountryRegion>((IEnumerable<BookKeeping.Domain.Models.CountryRegion>)cachedList, (Func<BookKeeping.Domain.Models.CountryRegion, bool>)(cr => cr.Id == countryRegion.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.CountryRegion>((IEnumerable<BookKeeping.Domain.Models.CountryRegion>)cachedList, (Func<BookKeeping.Domain.Models.CountryRegion, bool>)(cr => cr.Id != countryRegion.Id)))
                    return;
                cachedList.Add(countryRegion);
            }
        }

        private List<BookKeeping.Domain.Models.CountryRegion> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.CountryRegion> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.CountryRegion>>("CountryRegions-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.CountryRegion>>("CountryRegions-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.CountryRegion>(this._repository.GetAll(storeId, new long?()));
                        this._cacheService.SetCacheValue("CountryRegions-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}