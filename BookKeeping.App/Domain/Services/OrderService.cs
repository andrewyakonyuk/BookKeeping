using BookKeeping.App.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookKeeping.App.Domain.Repositories;
using BookKeeping.App.Infrastructure.Caching;

namespace BookKeeping.App.Domain.Services
{
   public class OrderService : IOrderService
    {
       readonly IOrderRepository _repository;
       readonly ICacheService _cache;

       public OrderService(IOrderRepository repository, ICacheService cache)
       {
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
