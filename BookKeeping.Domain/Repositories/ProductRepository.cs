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
    public class ProductRepository : RepositoryBase<Product, ProductId>, IRepository<Product, ProductId>
    {
        readonly IEventStore _eventStore;
        readonly IEventBus _eventBus;
        readonly IDocumentReader<unit, ProductIndexLookup> _userIndexReader;

        public ProductRepository(IEventStore eventStore, IEventBus eventBus, IDocumentReader<unit, ProductIndexLookup> userIndexReader)
            : base(eventStore, eventBus)
        {
            _eventStore = eventStore;
            _userIndexReader = userIndexReader;
            _eventBus = eventBus;
        }

        public override IEnumerable<Product> All()
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

        public override Product Get(ProductId id)
        {
            var stream = _eventStore.LoadEventStream(id);
            return new Product(stream.Events);
        }

        public override Product Load(ProductId id)
        {
            var stream = _eventStore.LoadEventStream(id);
            if (stream.Version > 0)
                return new Product(stream.Events);
            return null;
        }
    }
}
