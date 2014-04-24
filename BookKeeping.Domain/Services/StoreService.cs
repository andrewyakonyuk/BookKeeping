using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class StoreService : IStoreService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "Stores";
        private readonly IStoreRepository _repository;
        private readonly ICacheService _cacheService;

        public static IStoreService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IStoreService>();
            }
        }

        public StoreService(IStoreRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.Store.Created += new StoreEventHandler(this.StoreCreated);
        }

        public IEnumerable<BookKeeping.Domain.Models.Store> GetAll()
        {
            return (IEnumerable<BookKeeping.Domain.Models.Store>)Enumerable.OrderBy<BookKeeping.Domain.Models.Store, int>(Enumerable.Where<BookKeeping.Domain.Models.Store>((IEnumerable<BookKeeping.Domain.Models.Store>)this.GetCachedList(), (Func<BookKeeping.Domain.Models.Store, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.Store, int>)(i => i.Sort));
        }

        public BookKeeping.Domain.Models.Store Get(long storeId)
        {
            List<BookKeeping.Domain.Models.Store> cachedList = this.GetCachedList();
            BookKeeping.Domain.Models.Store store = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Store>((IEnumerable<BookKeeping.Domain.Models.Store>)cachedList, (Func<BookKeeping.Domain.Models.Store, bool>)(i => i.Id == storeId));
            if (store == null)
            {
                lock (this._cacheLock)
                {
                    store = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Store>((IEnumerable<BookKeeping.Domain.Models.Store>)cachedList, (Func<BookKeeping.Domain.Models.Store, bool>)(i => i.Id == storeId));
                    if (store == null)
                    {
                        store = this._repository.Get(storeId);
                        if (store != null)
                            cachedList.Add(store);
                    }
                }
            }
            return store;
        }

        private void StoreCreated(BookKeeping.Domain.Models.Store store)
        {
            List<BookKeeping.Domain.Models.Store> cachedList = this.GetCachedList();
            if (Enumerable.Any<BookKeeping.Domain.Models.Store>((IEnumerable<BookKeeping.Domain.Models.Store>)cachedList, (Func<BookKeeping.Domain.Models.Store, bool>)(s => s.Id == store.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.Store>((IEnumerable<BookKeeping.Domain.Models.Store>)cachedList, (Func<BookKeeping.Domain.Models.Store, bool>)(s => s.Id != store.Id)))
                    return;
                cachedList.Add(store);
            }
        }

        private List<BookKeeping.Domain.Models.Store> GetCachedList()
        {
            List<BookKeeping.Domain.Models.Store> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Store>>("Stores");
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Store>>("Stores");
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.Store>(this._repository.GetAll());
                        this._cacheService.SetCacheValue("Stores", (object)list);
                    }
                }
            }
            return list;
        }
    }
}