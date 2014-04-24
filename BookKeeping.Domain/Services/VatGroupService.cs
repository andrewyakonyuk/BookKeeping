using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class VatGroupService : IVatGroupService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "VatGroups";
        private readonly IVatGroupRepository _repository;
        private readonly ICacheService _cacheService;

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

        public IEnumerable<BookKeeping.Domain.Models.VatGroup> GetAll(long storeId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.VatGroup>)Enumerable.OrderBy<BookKeeping.Domain.Models.VatGroup, int>(Enumerable.Where<BookKeeping.Domain.Models.VatGroup>((IEnumerable<BookKeeping.Domain.Models.VatGroup>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.VatGroup, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.VatGroup, int>)(i => i.Sort));
        }

        public BookKeeping.Domain.Models.VatGroup Get(long storeId, long vatGroupId)
        {
            List<BookKeeping.Domain.Models.VatGroup> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.VatGroup vatGroup = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.VatGroup>((IEnumerable<BookKeeping.Domain.Models.VatGroup>)cachedList, (Func<BookKeeping.Domain.Models.VatGroup, bool>)(i => i.Id == vatGroupId));
            if (vatGroup == null)
            {
                lock (this._cacheLock)
                {
                    vatGroup = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.VatGroup>((IEnumerable<BookKeeping.Domain.Models.VatGroup>)cachedList, (Func<BookKeeping.Domain.Models.VatGroup, bool>)(i => i.Id == vatGroupId));
                    if (vatGroup == null)
                    {
                        vatGroup = this._repository.Get(storeId, vatGroupId);
                        if (vatGroup != null)
                            this.GetCachedList(storeId).Add(vatGroup);
                    }
                }
            }
            return vatGroup;
        }

        private void VatGroupCreated(BookKeeping.Domain.Models.VatGroup vatGroup)
        {
            List<BookKeeping.Domain.Models.VatGroup> cachedList = this.GetCachedList(vatGroup.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.VatGroup>((IEnumerable<BookKeeping.Domain.Models.VatGroup>)cachedList, (Func<BookKeeping.Domain.Models.VatGroup, bool>)(vg => vg.Id == vatGroup.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.VatGroup>((IEnumerable<BookKeeping.Domain.Models.VatGroup>)cachedList, (Func<BookKeeping.Domain.Models.VatGroup, bool>)(vg => vg.Id != vatGroup.Id)))
                    return;
                cachedList.Add(vatGroup);
            }
        }

        private List<BookKeeping.Domain.Models.VatGroup> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.VatGroup> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.VatGroup>>("VatGroups-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.VatGroup>>("VatGroups-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.VatGroup>(this._repository.GetAll(storeId));
                        this._cacheService.SetCacheValue("VatGroups-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}