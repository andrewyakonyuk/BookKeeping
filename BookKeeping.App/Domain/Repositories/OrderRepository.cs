using BookKeeping.App.Domain.Aggregates;
using System;

namespace BookKeeping.App.Domain.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public Maybe<Order> Get(long orderId)
        {
            throw new NotImplementedException();
        }

        public Order Load(long orderId)
        {
            throw new NotImplementedException();
        }

        public void Save(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
