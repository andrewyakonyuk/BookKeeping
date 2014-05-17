using BookKeeping.Domain.Aggregates;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Services
{
    public interface IProductService
    {
        Decimal GetStock(long productId);

        void SetStock(long productId, Decimal stock);

        Maybe<Product> Get(long productId);

        IEnumerable<Product> GetAll();
    }
}
