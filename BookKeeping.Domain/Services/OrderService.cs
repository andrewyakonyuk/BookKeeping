using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class OrderService : IOrderService
    {
        private const string CacheKey = "Orders";
        private readonly IOrderRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

        public static IOrderService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IOrderService>();
            }
        }

        public OrderService(IOrderRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.Order.Deleted += new OrderEventHandler(this.Order_Deleted);
        }

        public long? GetOrderId(long storeId, string cartNumber)
        {
            return this._repository.GetOrderId(storeId, cartNumber);
        }

        public Order Get(long storeId, long orderId)
        {
            List<Order> cachedList = this.GetCachedList(storeId);
            Order order = cachedList.SingleOrDefault((Order i) => i.Id == orderId);
            if (order == null)
            {
                lock (this._cacheLock)
                {
                    order = cachedList.SingleOrDefault((Order i) => i.Id == orderId);
                    if (order == null)
                    {
                        order = this._repository.Get(storeId, orderId);
                        if (order != null)
                        {
                            cachedList.Add(order);
                        }
                    }
                }
            }
            return order;
        }

        public IEnumerable<Order> Get(long storeId, IEnumerable<long> orderIds)
        {
            List<Order> orders = new List<Order>();
            if (orderIds != null)
            {
                orderIds = orderIds.ToList();
                List<Order> cachedOrders = this.GetCachedList(storeId);
                orders.AddRange(
                    from orderId in orderIds
                    select cachedOrders.SingleOrDefault((Order i) => i.Id == orderId) into order
                    where order != null
                    select order);
                List<long> list = (
                    from orderId in orderIds
                    where orders.All((Order o) => o.Id != orderId)
                    select orderId).ToList();
                if (list.Any<long>())
                {
                    lock (this._cacheLock)
                    {
                        list = (
                            from orderId in orderIds
                            where orders.All((Order o) => o.Id != orderId)
                            select orderId).ToList();
                        if (list.Any<long>())
                        {
                            List<Order> collection = this._repository.Get(storeId, list).ToList<Order>();
                            cachedOrders.AddRange(collection);
                            orders.AddRange(collection);
                        }
                    }
                }
            }
            return orders;
        }

        public Tuple<IEnumerable<Order>, long> Search(long storeId, long orderStatusId, string orderNumber, string firstName, string lastName, PaymentState? paymentState, DateTime? startDate, DateTime? endDate, bool? isFinalized, long page, long itemsPerPage)
        {
            List<Order> list = new List<Order>();
            Tuple<IEnumerable<Order>, long> tuple = this._repository.Search(storeId, orderStatusId, orderNumber, firstName, lastName, paymentState, startDate, endDate, isFinalized, page, itemsPerPage);
            List<Order> cachedList = this.GetCachedList(storeId);
            foreach (var dbOrder in tuple.Item1)
            {
                Order order = cachedList.SingleOrDefault((Order o) => o.Id == dbOrder.Id);
                if (order == null)
                {
                    lock (this._cacheLock)
                    {
                        order = cachedList.SingleOrDefault((Order o) => o.Id == dbOrder.Id);
                        if (order == null)
                        {
                            cachedList.Add(dbOrder);
                            order = dbOrder;
                        }
                    }
                }
                list.Add(order);
            }
            return Tuple.Create<IEnumerable<Order>, long>(list.AsEnumerable<Order>(), tuple.Item2);
        }

        private void Order_Deleted(Order order)
        {
            List<Order> cachedList = this.GetCachedList(order.StoreId);
            if (cachedList.All((Order o) => o.Id != order.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.Any((Order o) => o.Id == order.Id))
                {
                    cachedList.Remove(order);
                }
            }
        }

        private List<Order> GetCachedList(long storeId)
        {
            List<Order> list = this._cacheService.GetCacheValue<List<Order>>("Orders-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<Order>>("Orders-" + storeId);
                    if (list == null)
                    {
                        list = new List<Order>();
                        this._cacheService.SetCacheValue("Orders-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}