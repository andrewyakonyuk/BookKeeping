using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure;

namespace BookKeeping.Domain.Factories
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
