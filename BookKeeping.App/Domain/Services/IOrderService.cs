using BookKeeping.App.Domain.Aggregates;
using System.Collections.Generic;

namespace BookKeeping.App.Domain.Services
{
    public interface IOrderService
    {
        Maybe<Order> Get(long orderId);

        IEnumerable<Order> GetAll();
    }
}
