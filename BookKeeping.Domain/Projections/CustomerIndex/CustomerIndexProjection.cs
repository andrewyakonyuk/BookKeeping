using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;

namespace BookKeeping.Domain.Projections.CustomerIndex
{
       public sealed class CustomerIndexProjection :
        IEventHandler<CustomerCreated>,
        IEventHandler<CustomerDeleted>
    {
        readonly IDocumentWriter<unit, CustomerIndexLookup> _writer;

        public CustomerIndexProjection(IDocumentWriter<unit, CustomerIndexLookup> writer)
        {
            _writer = writer;
        }

        public void When(CustomerCreated e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Add(e.Id);
            });
        }

        public void When(CustomerDeleted e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Remove(e.Id);
            });
        }
    }
}
