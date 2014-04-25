using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class CurrencyService : ICurrencyService
    {
        private const string CacheKey = "Currencies";
        private readonly ICurrencyRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

        public static ICurrencyService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<ICurrencyService>();
            }
        }

        public CurrencyService(ICurrencyRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.Currency.Created += new CurrencyEventHandler(this.CurrencyCreated);
        }

        public IEnumerable<Currency> GetAll(long storeId)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public IEnumerable<Currency> GetAllAllowedIn(long storeId, long countryId)
        {
            return (
                from c in this.GetAll(storeId)
                where c.AllowedInFollowingCountries.Contains(countryId)
                select c).ToList<Currency>();
        }

        public Currency Get(long storeId, long currencyId)
        {
            List<Currency> cachedList = this.GetCachedList(storeId);
            Currency currency = cachedList.SingleOrDefault((Currency i) => i.Id == currencyId);
            if (currency == null)
            {
                lock (this._cacheLock)
                {
                    currency = cachedList.SingleOrDefault((Currency i) => i.Id == currencyId);
                    if (currency == null)
                    {
                        currency = this._repository.Get(storeId, currencyId);
                        if (currency != null)
                        {
                            cachedList.Add(currency);
                        }
                    }
                }
            }
            return currency;
        }

        private void CurrencyCreated(Currency currency)
        {
            List<Currency> cachedList = this.GetCachedList(currency.StoreId);
            if (cachedList.Any((Currency c) => c.Id == currency.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((Currency c) => c.Id != currency.Id))
                {
                    cachedList.Add(currency);
                }
            }
        }

        private List<Currency> GetCachedList(long storeId)
        {
            List<Currency> list = this._cacheService.GetCacheValue<List<Currency>>("Currencies-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<Currency>>("Currencies-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId).ToList<Currency>();
                        this._cacheService.SetCacheValue("Currencies-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}