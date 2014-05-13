using BookKeeping.App.Domain.Aggregates;
using BookKeeping.App.Domain.Services;
using BookKeeping.App.Infrastructure;

namespace BookKeeping.App.Domain.Factories
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
