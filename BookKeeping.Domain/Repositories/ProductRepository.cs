using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.ProductIndex;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public class ProductRepository : IRepository<Product, ProductId>
    {
        readonly IEventStore _eventStore;
        readonly IDocumentReader<unit, ProductIndexLookup> _userIndexReader;
        readonly IUnitOfWork _unitOfWork;

        public ProductRepository(IEventStore eventStore, IUnitOfWork unitOfWork, IDocumentReader<unit, ProductIndexLookup> userIndexReader)
        {
            _eventStore = eventStore;
            _userIndexReader = userIndexReader;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Product> All()
        {
            var index = _userIndexReader.Get<ProductIndexLookup>();
            if (index.HasValue)
            {
                foreach (var item in index.Value.Identities)
                {
                    yield return Get(item);
                }
            }
            yield break;
        }

        public Product Get(ProductId id)
        {
            Product product = null;
            product = _unitOfWork.Get<Product>(id);
            if (product == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                product = new Product(stream.Events);
                _unitOfWork.RegisterForTracking(product, id);
            }
            return product;
        }

        public Product Load(ProductId id)
        {
            Product product = null;
            product = _unitOfWork.Get<Product>(id);
            if (product == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                if (stream.Version > 0)
                {
                    product = new Product(stream.Events);
                    _unitOfWork.RegisterForTracking(product, id);
                    return product;
                }
            }
            return null;
        }
    }
}
