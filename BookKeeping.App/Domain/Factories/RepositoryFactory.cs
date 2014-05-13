using BookKeeping.App.Domain.Repositories;
using BookKeeping.App.Infrastructure;

namespace BookKeeping.App.Domain.Factories
{
    public class RepositoryFactory : AbstractFactory,
        IFactoryMethod<IProductRepository>,
        IFactoryMethod<IOrderRepository>
    {
        IOrderRepository IFactoryMethod<IOrderRepository>.Create()
        {
            return new OrderRepository();
        }

        IProductRepository IFactoryMethod<IProductRepository>.Create()
        {
            return new ProductRepository();
        }
    }
}
