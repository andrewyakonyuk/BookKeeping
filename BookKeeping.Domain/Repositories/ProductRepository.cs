using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Aggregates;

namespace BookKeeping.Domain.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public decimal GetStock(long productId)
        {
            return Get(productId).Convert(t => t.Stock, 0);
        }

        public void SetStock(long productId, decimal stock)
        {
            Get(productId).IfValue(t => t.Stock = stock);
        }

        public Maybe<Product> Get(long productId)
        {
            return Maybe<Product>.Empty;
        }

        public IEnumerable<Product> GetAll()
        {
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                yield return new Product
                {
                    Id = i,
                    Barcode = Barcode.Undefined,
                    IsOrderable = true,
                    ItemNo = "item no. " + (i + 1),
                    Price = new CurrencyAmount(random.Next(10, 100), Currency.Eur),
                    Stock = random.Next(1, 1000),
                    Title = new string("qwertyuiopasdfghjklzxcvbnm".Substring(random.Next(0, 12)).OrderBy(t => Guid.NewGuid()).ToArray()),
                    UnitOfMeasure = "m2",
                    VatRate = new VatRate(random.Next(0, 50))
                };
            }
           
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
