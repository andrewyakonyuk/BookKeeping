using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class CountryService : ICountryService
    {
        private const string CacheKey = "Countries";
        private readonly ICountryRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

        public static ICountryService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<ICountryService>();
            }
        }

        public CountryService(ICountryRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.Country.Created += new CountryEventHandler(this.CountryCreated);
        }

        public IEnumerable<Country> GetAll(long storeId)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public Country Get(long storeId, long countryId)
        {
            List<Country> cachedList = this.GetCachedList(storeId);
            Country country = cachedList.SingleOrDefault((Country i) => i.Id == countryId);
            if (country == null)
            {
                lock (this._cacheLock)
                {
                    country = cachedList.SingleOrDefault((Country i) => i.Id == countryId);
                    if (country == null)
                    {
                        country = this._repository.Get(storeId, countryId);
                        if (country != null)
                        {
                            cachedList.Add(country);
                        }
                    }
                }
            }
            return country;
        }

        private void CountryCreated(Country country)
        {
            List<Country> cachedList = this.GetCachedList(country.StoreId);
            if (cachedList.Any((Country c) => c.Id == country.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((Country c) => c.Id != country.Id))
                {
                    cachedList.Add(country);
                }
            }
        }

        private List<Country> GetCachedList(long storeId)
        {
            List<Country> list = this._cacheService.GetCacheValue<List<Country>>("Countries-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<Country>>("Countries-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId).ToList<Country>();
                        this._cacheService.SetCacheValue("Countries-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}