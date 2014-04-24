using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "OrderStatuses";
        private readonly IOrderStatusRepository _repository;
        private readonly ICacheService _cacheService;

        public static IOrderStatusService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IOrderStatusService>();
            }
        }

        public OrderStatusService(IOrderStatusRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.OrderStatus.Created += new OrderStatusEventHandler(this.OrderStatusCreated);
        }

        public IEnumerable<BookKeeping.Domain.Models.OrderStatus> GetAll(long storeId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.OrderStatus>)Enumerable.OrderBy<BookKeeping.Domain.Models.OrderStatus, int>(Enumerable.Where<BookKeeping.Domain.Models.OrderStatus>((IEnumerable<BookKeeping.Domain.Models.OrderStatus>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.OrderStatus, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.OrderStatus, int>)(i => i.Sort));
        }

        public BookKeeping.Domain.Models.OrderStatus Get(long storeId, long orderStatusId)
        {
            List<BookKeeping.Domain.Models.OrderStatus> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.OrderStatus orderStatus = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.OrderStatus>((IEnumerable<BookKeeping.Domain.Models.OrderStatus>)cachedList, (Func<BookKeeping.Domain.Models.OrderStatus, bool>)(i => i.Id == orderStatusId));
            if (orderStatus == null)
            {
                lock (this._cacheLock)
                {
                    orderStatus = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.OrderStatus>((IEnumerable<BookKeeping.Domain.Models.OrderStatus>)cachedList, (Func<BookKeeping.Domain.Models.OrderStatus, bool>)(i => i.Id == orderStatusId));
                    if (orderStatus == null)
                    {
                        orderStatus = this._repository.Get(storeId, orderStatusId);
                        if (orderStatus != null)
                            cachedList.Add(orderStatus);
                    }
                }
            }
            return orderStatus;
        }

        private void OrderStatusCreated(BookKeeping.Domain.Models.OrderStatus orderStatus)
        {
            List<BookKeeping.Domain.Models.OrderStatus> cachedList = this.GetCachedList(orderStatus.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.OrderStatus>((IEnumerable<BookKeeping.Domain.Models.OrderStatus>)cachedList, (Func<BookKeeping.Domain.Models.OrderStatus, bool>)(o => o.Id == orderStatus.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.OrderStatus>((IEnumerable<BookKeeping.Domain.Models.OrderStatus>)cachedList, (Func<BookKeeping.Domain.Models.OrderStatus, bool>)(o => o.Id != orderStatus.Id)))
                    return;
                cachedList.Add(orderStatus);
            }
        }

        private List<BookKeeping.Domain.Models.OrderStatus> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.OrderStatus> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.OrderStatus>>("OrderStatuses-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.OrderStatus>>("OrderStatuses-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.OrderStatus>(this._repository.GetAll(storeId));
                        this._cacheService.SetCacheValue("OrderStatuses-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}