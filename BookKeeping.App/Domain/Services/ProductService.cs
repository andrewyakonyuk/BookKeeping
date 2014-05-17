using BookKeeping.App.Domain.Aggregates;
using BookKeeping.App.Domain.Repositories;
using BookKeeping.App.Infrastructure.Caching;
using System.Collections.Generic;

namespace BookKeeping.App.Domain.Services
{
    public class ProductService : IProductService
    {
        readonly IProductRepository _repository;
        readonly ICacheService _cache;

        public ProductService(IProductRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public decimal GetStock(long productId)
        {
            return this._repository.GetStock(productId);
        }

        public void SetStock(long productId, decimal stock)
        {
            this._repository.SetStock(productId, stock);
        }

        public Maybe<Product> Get(long productId)
        {
            var cacheKey = "Product::" + productId;
            return _cache.Get<Maybe<Product>>(cacheKey, () => this._repository.Get(productId));
        }

        public IEnumerable<Product> GetAll()
        {
            var cacheKey = "Product::all";
            return _cache.Get<IEnumerable<Product>>(cacheKey, () => this._repository.GetAll());
        }
    }
}
