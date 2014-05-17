using BookKeeping.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookKeeping.Domain.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        public Maybe<Order> Get(long orderId)
        {
           return Maybe<Order>.Empty;
        }

        public IEnumerable<Order> GetAll()
        {
            return Enumerable.Empty<Order>();
        }

        public void Dispose()
        {
        }
    }
}
