using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.VendorIndex;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public class VendorRepository : IRepository<Vendor, VendorId>
    {
        readonly IEventStore _eventStore;
        readonly IDocumentReader<unit, VendorIndexLookup> _customerIndexReader;
        readonly IUnitOfWork _unitOfWork;

        public VendorRepository(IEventStore eventStore, IUnitOfWork unitOfWork, IDocumentReader<unit, VendorIndexLookup> customerIndexReader)
        {
            _eventStore = eventStore;
            _customerIndexReader = customerIndexReader;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Vendor> All()
        {
            var index = _customerIndexReader.Get<VendorIndexLookup>();
            if (index.HasValue)
            {
                foreach (var item in index.Value.Identities)
                {
                    yield return Get(item);
                }
            }
            yield break;
        }

        public Vendor Get(VendorId id)
        {
            Vendor vendor = null;
            vendor = _unitOfWork.Get<Vendor>(id);
            if (vendor == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                vendor = new Vendor(stream.Events);
                _unitOfWork.RegisterForTracking(vendor, id);
            }
            return vendor;
        }

        public Vendor Load(VendorId id)
        {
            Vendor vendor = null;
            vendor = _unitOfWork.Get<Vendor>(id);
            if (vendor == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                if (stream.Version > 0)
                {
                    vendor = new Vendor(stream.Events);
                    _unitOfWork.RegisterForTracking(vendor, id);
                    return vendor;
                }
            }
            return null;
        }
    }
}
