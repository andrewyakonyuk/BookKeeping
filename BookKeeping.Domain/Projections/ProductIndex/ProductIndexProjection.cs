using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;

namespace BookKeeping.Domain.Projections.ProductIndex
{
    public sealed class ProductIndexProjection :
        IEventHandler<ProductCreated>,
        IEventHandler<ProductDeleted>
    {
        readonly IDocumentWriter<unit, ProductIndexLookup> _writer;

        public ProductIndexProjection(IDocumentWriter<unit, ProductIndexLookup> writer)
        {
            _writer = writer;
        }

        public void When(ProductCreated e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Add(e.Id);
            });
        }

        public void When(ProductDeleted e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Remove(e.Id);
            });
        }
    }
}
