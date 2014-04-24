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
        private readonly object _cacheLock = new object();
        private const string CacheKey = "Orders";
        private readonly IOrderRepository _repository;
        private readonly ICacheService _cacheService;

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

        public Guid? GetOrderId(long storeId, string cartNumber)
        {
            return this._repository.GetOrderId(storeId, cartNumber);
        }

        public XElement GetAllFinalizedAsXml(long storeId)
        {
            return this._repository.GetAllFinalizedAsXml(storeId);
        }

        public BookKeeping.Domain.Models.Order Get(long storeId, Guid orderId)
        {
            List<BookKeeping.Domain.Models.Order> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.Order order = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedList, (Func<BookKeeping.Domain.Models.Order, bool>)(i => i.Id == orderId));
            if (order == null)
            {
                lock (this._cacheLock)
                {
                    order = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedList, (Func<BookKeeping.Domain.Models.Order, bool>)(i => i.Id == orderId));
                    if (order == null)
                    {
                        order = this._repository.Get(storeId, orderId);
                        if (order != null)
                            cachedList.Add(order);
                    }
                }
            }
            return order;
        }

        public IEnumerable<BookKeeping.Domain.Models.Order> Get(long storeId, IEnumerable<Guid> orderIds)
        {
            List<BookKeeping.Domain.Models.Order> orders = new List<BookKeeping.Domain.Models.Order>();
            if (orderIds != null)
            {
                orderIds = (IEnumerable<Guid>)Enumerable.ToList<Guid>(orderIds);
                List<BookKeeping.Domain.Models.Order> cachedOrders = this.GetCachedList(storeId);
                orders.AddRange(Enumerable.Where<BookKeeping.Domain.Models.Order>(Enumerable.Select<Guid, BookKeeping.Domain.Models.Order>(orderIds, (Func<Guid, BookKeeping.Domain.Models.Order>)(orderId => Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedOrders, (Func<BookKeeping.Domain.Models.Order, bool>)(i => i.Id == orderId)))), (Func<BookKeeping.Domain.Models.Order, bool>)(order => order != null)));
                if (Enumerable.Any<Guid>((IEnumerable<Guid>)Enumerable.ToList<Guid>(Enumerable.Where<Guid>(orderIds, (Func<Guid, bool>)(orderId => Enumerable.All<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)orders, (Func<BookKeeping.Domain.Models.Order, bool>)(o => o.Id != orderId)))))))
                {
                    lock (this._cacheLock)
                    {
                        List<Guid> local_0_1 = Enumerable.ToList<Guid>(Enumerable.Where<Guid>(orderIds, (Func<Guid, bool>)(orderId => Enumerable.All<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)orders, (Func<BookKeeping.Domain.Models.Order, bool>)(o => o.Id != orderId)))));
                        if (Enumerable.Any<Guid>((IEnumerable<Guid>)local_0_1))
                        {
                            List<BookKeeping.Domain.Models.Order> local_1 = Enumerable.ToList<BookKeeping.Domain.Models.Order>(this._repository.Get(storeId, (IEnumerable<Guid>)local_0_1));
                            cachedOrders.AddRange((IEnumerable<BookKeeping.Domain.Models.Order>)local_1);
                            orders.AddRange((IEnumerable<BookKeeping.Domain.Models.Order>)local_1);
                        }
                    }
                }
            }
            return (IEnumerable<BookKeeping.Domain.Models.Order>)orders;
        }

        public Tuple<IEnumerable<BookKeeping.Domain.Models.Order>, long> Search(long storeId, long orderStatusId, string orderNumber, string firstName, string lastName, PaymentState? paymentState, DateTime? startDate, DateTime? endDate, bool? isFinalized, long page, long itemsPerPage)
        {
            List<BookKeeping.Domain.Models.Order> list = new List<BookKeeping.Domain.Models.Order>();
            Tuple<IEnumerable<BookKeeping.Domain.Models.Order>, long> tuple = this._repository.Search(storeId, orderStatusId, orderNumber, firstName, lastName, paymentState, startDate, endDate, isFinalized, page, itemsPerPage);
            List<BookKeeping.Domain.Models.Order> cachedList = this.GetCachedList(storeId);
            foreach (BookKeeping.Domain.Models.Order order1 in tuple.Item1)
            {
                BookKeeping.Domain.Models.Order dbOrder = order1;
                BookKeeping.Domain.Models.Order order2 = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedList, (Func<BookKeeping.Domain.Models.Order, bool>)(o => o.Id == dbOrder.Id));
                if (order2 == null)
                {
                    lock (this._cacheLock)
                    {
                        order2 = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedList, (Func<BookKeeping.Domain.Models.Order, bool>)(o => o.Id == dbOrder.Id));
                        if (order2 == null)
                        {
                            cachedList.Add(dbOrder);
                            order2 = dbOrder;
                        }
                    }
                }
                list.Add(order2);
            }
            return Tuple.Create<IEnumerable<BookKeeping.Domain.Models.Order>, long>(Enumerable.AsEnumerable<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)list), tuple.Item2);
        }

        private void Order_Deleted(BookKeeping.Domain.Models.Order order)
        {
            List<BookKeeping.Domain.Models.Order> cachedList = this.GetCachedList(order.StoreId);
            if (Enumerable.All<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedList, (Func<BookKeeping.Domain.Models.Order, bool>)(o => o.Id != order.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.Any<BookKeeping.Domain.Models.Order>((IEnumerable<BookKeeping.Domain.Models.Order>)cachedList, (Func<BookKeeping.Domain.Models.Order, bool>)(o => o.Id == order.Id)))
                    return;
                cachedList.Remove(order);
            }
        }

        private List<BookKeeping.Domain.Models.Order> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.Order> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Order>>("Orders-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.Order>>("Orders-" + (object)storeId);
                    if (list == null)
                    {
                        list = new List<BookKeeping.Domain.Models.Order>();
                        this._cacheService.SetCacheValue("Orders-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}