using BookKeeping.App.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.Domain.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public decimal GetStock(string itemNo)
        {
            throw new NotImplementedException();
        }

        public void SetStock(string itemNo, decimal stock)
        {
            throw new NotImplementedException();
        }

        public Maybe<Product> Get(string itemNo)
        {
            throw new NotImplementedException();
        }

        public void Save(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
