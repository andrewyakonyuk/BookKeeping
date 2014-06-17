using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;

namespace BookKeeping.Domain.Projections.VendorIndex
{
       public sealed class VendorIndexProjection :
        IEventHandler<VendorCreated>,
        IEventHandler<VendorDeleted>
    {
        readonly IDocumentWriter<unit, VendorIndexLookup> _writer;

        public VendorIndexProjection(IDocumentWriter<unit, VendorIndexLookup> writer)
        {
            _writer = writer;
        }

        public void When(VendorCreated e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Add(e.Id);
            });
        }

        public void When(VendorDeleted e)
        {
            _writer.UpdateEnforcingNew(unit.it, si =>
            {
                si.Identities.Remove(e.Id);
            });
        }
    }
}
