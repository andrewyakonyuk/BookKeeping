using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure;

namespace BookKeeping.Domain.Factories
{
    public class AggregateFactory : AbstractFactory,
        IFactoryMethod<Product>,
        IFactoryMethod<Order>
    {
        static ServiceFactory _serviceFactory = new ServiceFactory();

        Product IFactoryMethod<Product>.Create()
        {
            return new Product();
        }

        Order IFactoryMethod<Order>.Create()
        {
            return new Order(_serviceFactory.Create<IOrderCalculator>(), _serviceFactory.Create<ProductService>());
        }
    }
}
