using BookKeeping.App.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BookKeeping.App.Domain.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public Maybe<Order> Get(long orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
