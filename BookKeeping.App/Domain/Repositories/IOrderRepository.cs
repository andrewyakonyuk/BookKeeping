using BookKeeping.App.Domain.Aggregates;

namespace BookKeeping.App.Domain.Repositories
{
    public interface IOrderRepository
    {
        Maybe<Order> Get(long orderId);
        Order Load(long orderId);
        void Save(Order order);
    }
}
