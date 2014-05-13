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
        static RepositoryFactory _repositoryFactory;
        static IOrderCalculator _orderCalculator;
        static IOrderService _orderService;
        static IProductService _productService;

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
            if (_orderService == null)
            {
                _orderService = new OrderService(_repositoryFactory.Create<IOrderRepository>(), CacheService.Current);
            }
            return _orderService;
        }

        IProductService IFactoryMethod<IProductService>.Create()
        {
            if (_productService == null)
            {
                _productService = new ProductService(_repositoryFactory.Create<IProductRepository>(), CacheService.Current);
            }
            return _productService;
        }
    }
}
