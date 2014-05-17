using BookKeeping.App.Domain.Aggregates;
using System;
using System.Collections.Generic;

namespace BookKeeping.App.Domain.Repositories
{
    public interface IOrderRepository : IDisposable
    {
        Maybe<Order> Get(long orderId);
        IEnumerable<Order> GetAll();
    }
}
