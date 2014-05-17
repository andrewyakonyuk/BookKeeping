using BookKeeping.Domain.Aggregates;
using System.Collections.Generic;

namespace BookKeeping.Domain.Services
{
    public interface IOrderService
    {
        Maybe<Order> Get(long orderId);

        IEnumerable<Order> GetAll();
    }
}
