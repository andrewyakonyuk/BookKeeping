using BookKeeping.App.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.Domain.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public decimal GetStock(long productId)
        {
            throw new NotImplementedException();
        }

        public void SetStock(long productId, decimal stock)
        {
            throw new NotImplementedException();
        }

        public Maybe<Product> Get(long productId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
