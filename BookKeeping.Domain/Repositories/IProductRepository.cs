using BookKeeping.Domain.Aggregates;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public interface IProductRepository : IDisposable
    {
        decimal GetStock(long productId);

        void SetStock(long productId, decimal stock);

        Maybe<Product> Get(long productId);

        IEnumerable<Product> GetAll();
    }
}
