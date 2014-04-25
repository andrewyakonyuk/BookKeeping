using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class StoreService : IStoreService
    {
        private const string CacheKey = "Stores";
        private readonly IStoreRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

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

        public IEnumerable<Store> GetAll()
        {
            return
                from i in this.GetCachedList()
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public Store Get(long storeId)
        {
            List<Store> cachedList = this.GetCachedList();
            Store store = cachedList.SingleOrDefault((Store i) => i.Id == storeId);
            if (store == null)
            {
                lock (this._cacheLock)
                {
                    store = cachedList.SingleOrDefault((Store i) => i.Id == storeId);
                    if (store == null)
                    {
                        store = this._repository.Get(storeId);
                        if (store != null)
                        {
                            cachedList.Add(store);
                        }
                    }
                }
            }
            return store;
        }

        private void StoreCreated(Store store)
        {
            List<Store> cachedList = this.GetCachedList();
            if (cachedList.Any((Store s) => s.Id == store.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((Store s) => s.Id != store.Id))
                {
                    cachedList.Add(store);
                }
            }
        }

        private List<Store> GetCachedList()
        {
            List<Store> list = this._cacheService.GetCacheValue<List<Store>>("Stores");
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<Store>>("Stores");
                    if (list == null)
                    {
                        list = this._repository.GetAll().ToList<Store>();
                        this._cacheService.SetCacheValue("Stores", list);
                    }
                }
            }
            return list;
        }
    }
}