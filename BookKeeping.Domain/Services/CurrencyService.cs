using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "Currencies";
        private readonly ICurrencyRepository _repository;
        private readonly ICacheService _cacheService;

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

        public IEnumerable<BookKeeping.Domain.Models.Currency> GetAll(long storeId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.Currency>)Enumerable.OrderBy<BookKeeping.Domain.Models.Currency, int>(Enumerable.Where<BookKeeping.Domain.Models.Currency>((IEnumerable<BookKeeping.Domain.Models.Currency>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.Currency, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.Currency, int>)(i => i.Sort));
        }

        public IEnumerable<BookKeeping.Domain.Models.Currency> GetAllAllowedIn(long storeId, long countryId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.Currency>)Enumerable.ToList<BookKeeping.Domain.Models.Currency>(Enumerable.Where<BookKeeping.Domain.Models.Currency>(this.GetAll(storeId), (Func<BookKeeping.Domain.Models.Currency, bool>)(c => c.AllowedInFollowingCountries.Contains(countryId))));
        }

        public BookKeeping.Domain.Models.Currency Get(long storeId, long currencyId)
        {
            List<BookKeeping.Domain.Models.Currency> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.Currency currency = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Currency>((IEnumerable<BookKeeping.Domain.Models.Currency>)cachedList, (Func<BookKeeping.Domain.Models.Currency, bool>)(i => i.Id == currencyId));
            if (currency == null)
            {
                lock (this._cacheLock)
                {
                    currency = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Currency>((IEnumerable<BookKeeping.Domain.Models.Currency>)cachedList, (Func<BookKeeping.Domain.Models.Currency, bool>)(i => i.Id == currencyId));
                    if (currency == null)
                    {
                        currency = this._repository.Get(storeId, currencyId);
                        if (currency != null)
                            cachedList.Add(currency);
                    }
                }
            }
            return currency;
        }

        private void CurrencyCreated(BookKeeping.Domain.Models.Currency currency)
        {
            List<BookKeeping.Domain.Models.Currency> cachedList = this.GetCachedList(currency.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.Currency>((IEnumerable<BookKeeping.Domain.Models.Currency>)cachedList, (Func<BookKeeping.Domain.Models.Currency, bool>)(c => c.Id == currency.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.Currency>((IEnumerable<BookKeeping.Domain.Models.Currency>)cachedList, (Func<BookKeeping.Domain.Models.Currency, bool>)(c => c.Id != currency.Id)))
                    return;
                cachedList.Add(currency);
            }
        }

        private List<BookKeeping.Domain.Models.Currency> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.Currency> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Currency>>("Currencies-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Currency>>("Currencies-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.Currency>(this._repository.GetAll(storeId));
                        this._cacheService.SetCacheValue("Currencies-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}