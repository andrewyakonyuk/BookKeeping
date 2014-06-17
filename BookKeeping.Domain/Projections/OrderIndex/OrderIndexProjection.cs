using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;

namespace BookKeeping.Domain.Projections.OrderIndex
{
    public sealed class OrderIndexProjection :
        IEventHandler<OrderCreated>
    {
        readonly IDocumentWriter<unit, OrderIndexLookup> _writer;

        public OrderIndexProjection(IDocumentWriter<unit, OrderIndexLookup> writer)
        {
            _writer = writer;
        }

        public void When(OrderCreated e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Add(e.Id);
            });
        }
    }
}
