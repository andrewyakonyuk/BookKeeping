using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.ProductIndex;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
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
            var stream = _eventStore.LoadEventStream(id);
            var product = new Product(stream.Events);
            _unitOfWork.RegisterForTracking(product, id);
            return product;
        }

        public Product Load(ProductId id)
        {
            var stream = _eventStore.LoadEventStream(id);
            if (stream.Version > 0)
            {
                var product = new Product(stream.Events);
                _unitOfWork.RegisterForTracking(product, id);
                return product;
            }
            return null;
        }
    }
}
