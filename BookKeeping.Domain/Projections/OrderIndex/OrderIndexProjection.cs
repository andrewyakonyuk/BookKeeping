using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;

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
