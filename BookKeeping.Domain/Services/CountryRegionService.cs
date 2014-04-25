using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class CountryRegionService : ICountryRegionService
    {
        private const string CacheKey = "CountryRegions";
        private readonly ICountryRegionRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

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

        public IEnumerable<CountryRegion> GetAll(long storeId, long? countryId = null)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted && (!countryId.HasValue || i.CountryId == countryId)
                orderby i.Sort
                select i;
        }

        public CountryRegion Get(long storeId, long countryRegionId)
        {
            List<CountryRegion> cachedList = this.GetCachedList(storeId);
            CountryRegion countryRegion = cachedList.SingleOrDefault((CountryRegion i) => i.Id == countryRegionId);
            if (countryRegion == null)
            {
                lock (this._cacheLock)
                {
                    countryRegion = cachedList.SingleOrDefault((CountryRegion i) => i.Id == countryRegionId);
                    if (countryRegion == null)
                    {
                        countryRegion = this._repository.Get(storeId, countryRegionId);
                        if (countryRegion != null)
                        {
                            cachedList.Add(countryRegion);
                        }
                    }
                }
            }
            return countryRegion;
        }

        private void CountryRegionCreated(CountryRegion countryRegion)
        {
            List<CountryRegion> cachedList = this.GetCachedList(countryRegion.StoreId);
            if (cachedList.Any((CountryRegion cr) => cr.Id == countryRegion.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((CountryRegion cr) => cr.Id != countryRegion.Id))
                {
                    cachedList.Add(countryRegion);
                }
            }
        }

        private List<CountryRegion> GetCachedList(long storeId)
        {
            List<CountryRegion> list = this._cacheService.GetCacheValue<List<CountryRegion>>("CountryRegions-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<CountryRegion>>("CountryRegions-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId, null).ToList<CountryRegion>();
                        this._cacheService.SetCacheValue("CountryRegions-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}