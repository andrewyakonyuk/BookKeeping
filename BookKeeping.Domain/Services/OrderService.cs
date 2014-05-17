using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;

namespace BookKeeping.Domain.Services
{
   public class OrderService : IOrderService
    {
       readonly IOrderRepository _repository;
       readonly ICacheService _cache;

       public OrderService(IOrderRepository repository, ICacheService cache)
       {
           Contract.Requires<ArgumentNullException>(repository != null);
           Contract.Requires<ArgumentNullException>(cache != null);
           _repository = repository;
           _cache = cache;
       }

        public Maybe<Order> Get(long orderId)
        {
            var cacheKey = "Order::" + orderId;
            return _cache.Get<Maybe<Order>>(cacheKey, () => _repository.Get(orderId));
        }

        public IEnumerable<Order> GetAll()
        {
            var cacheKey = "Order::all";
            return _cache.Get<IEnumerable<Order>>(cacheKey, () => _repository.GetAll());
        }
    }
}
