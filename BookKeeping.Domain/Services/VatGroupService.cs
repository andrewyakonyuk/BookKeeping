using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class VatGroupService : IVatGroupService
    {
        private const string CacheKey = "VatGroups";
        private readonly IVatGroupRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

        public static IVatGroupService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IVatGroupService>();
            }
        }

        public VatGroupService(IVatGroupRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.VatGroup.Created += new VatGroupEventHandler(this.VatGroupCreated);
        }

        public IEnumerable<VatGroup> GetAll(long storeId)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public VatGroup Get(long storeId, long vatGroupId)
        {
            List<VatGroup> cachedList = this.GetCachedList(storeId);
            VatGroup vatGroup = cachedList.SingleOrDefault((VatGroup i) => i.Id == vatGroupId);
            if (vatGroup == null)
            {
                lock (this._cacheLock)
                {
                    vatGroup = cachedList.SingleOrDefault((VatGroup i) => i.Id == vatGroupId);
                    if (vatGroup == null)
                    {
                        vatGroup = this._repository.Get(storeId, vatGroupId);
                        if (vatGroup != null)
                        {
                            this.GetCachedList(storeId).Add(vatGroup);
                        }
                    }
                }
            }
            return vatGroup;
        }

        private void VatGroupCreated(VatGroup vatGroup)
        {
            List<VatGroup> cachedList = this.GetCachedList(vatGroup.StoreId);
            if (cachedList.Any((VatGroup vg) => vg.Id == vatGroup.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((VatGroup vg) => vg.Id != vatGroup.Id))
                {
                    cachedList.Add(vatGroup);
                }
            }
        }

        private List<VatGroup> GetCachedList(long storeId)
        {
            List<VatGroup> list = this._cacheService.GetCacheValue<List<VatGroup>>("VatGroups-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<VatGroup>>("VatGroups-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId).ToList<VatGroup>();
                        this._cacheService.SetCacheValue("VatGroups-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}