using BookKeeping.App.Domain.Repositories;
using BookKeeping.App.Domain.Services;
using BookKeeping.App.Infrastructure;
using BookKeeping.App.Infrastructure.Caching;

namespace BookKeeping.App.Domain.Factories
{
    public class ServiceFactory : AbstractFactory,
        IFactoryMethod<IOrderCalculator>,
        IFactoryMethod<IOrderService>,
        IFactoryMethod<IProductService>
    {
        static RepositoryFactory _repositoryFactory = new RepositoryFactory();
        static IOrderCalculator _orderCalculator;

        IOrderCalculator IFactoryMethod<IOrderCalculator>.Create()
        {
            if (_orderCalculator == null)
            {
                _orderCalculator = new OrderCalculator();
            }
            return _orderCalculator;
        }

        IOrderService IFactoryMethod<IOrderService>.Create()
        {
            using (var repository = _repositoryFactory.Create<IOrderRepository>())
            {
                return new OrderService(repository, CacheService.Current);
            }
        }

        IProductService IFactoryMethod<IProductService>.Create()
        {
            using (var repository = _repositoryFactory.Create<IProductRepository>())
            {
                return new ProductService(repository, CacheService.Current);
            }
        }
    }
}
