using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class CountryService : ICountryService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "Countries";
        private readonly ICountryRepository _repository;
        private readonly ICacheService _cacheService;

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

        public IEnumerable<BookKeeping.Domain.Models.Country> GetAll(long storeId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.Country>)Enumerable.OrderBy<BookKeeping.Domain.Models.Country, int>(Enumerable.Where<BookKeeping.Domain.Models.Country>((IEnumerable<BookKeeping.Domain.Models.Country>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.Country, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.Country, int>)(i => i.Sort));
        }

        public BookKeeping.Domain.Models.Country Get(long storeId, long countryId)
        {
            List<BookKeeping.Domain.Models.Country> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.Country country = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Country>((IEnumerable<BookKeeping.Domain.Models.Country>)cachedList, (Func<BookKeeping.Domain.Models.Country, bool>)(i => i.Id == countryId));
            if (country == null)
            {
                lock (this._cacheLock)
                {
                    country = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Country>((IEnumerable<BookKeeping.Domain.Models.Country>)cachedList, (Func<BookKeeping.Domain.Models.Country, bool>)(i => i.Id == countryId));
                    if (country == null)
                    {
                        country = this._repository.Get(storeId, countryId);
                        if (country != null)
                            cachedList.Add(country);
                    }
                }
            }
            return country;
        }

        private void CountryCreated(BookKeeping.Domain.Models.Country country)
        {
            List<BookKeeping.Domain.Models.Country> cachedList = this.GetCachedList(country.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.Country>((IEnumerable<BookKeeping.Domain.Models.Country>)cachedList, (Func<BookKeeping.Domain.Models.Country, bool>)(c => c.Id == country.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.Country>((IEnumerable<BookKeeping.Domain.Models.Country>)cachedList, (Func<BookKeeping.Domain.Models.Country, bool>)(c => c.Id != country.Id)))
                    return;
                cachedList.Add(country);
            }
        }

        private List<BookKeeping.Domain.Models.Country> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.Country> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Country>>("Countries-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Country>>("Countries-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.Country>(this._repository.GetAll(storeId));
                        this._cacheService.SetCacheValue("Countries-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}