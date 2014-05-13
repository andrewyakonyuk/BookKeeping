using BookKeeping.App.Domain.Aggregates;
using System.Collections.Generic;

namespace BookKeeping.App.Domain.Repositories
{
    public interface IProductRepository
    {
        decimal GetStock(long productId);

        void SetStock(long productId, decimal stock);

        Maybe<Product> Get(long productId);

        IEnumerable<Product> GetAll();
    }
}
