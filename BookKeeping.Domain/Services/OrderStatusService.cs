using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private const string CacheKey = "OrderStatuses";
        private readonly IOrderStatusRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

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

        public IEnumerable<OrderStatus> GetAll(long storeId)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public OrderStatus Get(long storeId, long orderStatusId)
        {
            List<OrderStatus> cachedList = this.GetCachedList(storeId);
            OrderStatus orderStatus = cachedList.SingleOrDefault((OrderStatus i) => i.Id == orderStatusId);
            if (orderStatus == null)
            {
                lock (this._cacheLock)
                {
                    orderStatus = cachedList.SingleOrDefault((OrderStatus i) => i.Id == orderStatusId);
                    if (orderStatus == null)
                    {
                        orderStatus = this._repository.Get(storeId, orderStatusId);
                        if (orderStatus != null)
                        {
                            cachedList.Add(orderStatus);
                        }
                    }
                }
            }
            return orderStatus;
        }

        private void OrderStatusCreated(OrderStatus orderStatus)
        {
            List<OrderStatus> cachedList = this.GetCachedList(orderStatus.StoreId);
            if (cachedList.Any((OrderStatus o) => o.Id == orderStatus.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((OrderStatus o) => o.Id != orderStatus.Id))
                {
                    cachedList.Add(orderStatus);
                }
            }
        }

        private List<OrderStatus> GetCachedList(long storeId)
        {
            List<OrderStatus> list = this._cacheService.GetCacheValue<List<OrderStatus>>("OrderStatuses-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<OrderStatus>>("OrderStatuses-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId).ToList<OrderStatus>();
                        this._cacheService.SetCacheValue("OrderStatuses-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}