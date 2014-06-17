using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.CustomerIndex;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public class CustomerRepository : IRepository<Customer, CustomerId>
    {
        readonly IEventStore _eventStore;
        readonly IDocumentReader<unit, CustomerIndexLookup> _customerIndexReader;
        readonly IUnitOfWork _unitOfWork;

        public CustomerRepository(IEventStore eventStore, IUnitOfWork unitOfWork, IDocumentReader<unit, CustomerIndexLookup> customerIndexReader)
        {
            _eventStore = eventStore;
            _customerIndexReader = customerIndexReader;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Customer> All()
        {
            var index = _customerIndexReader.Get<CustomerIndexLookup>();
            if (index.HasValue)
            {
                foreach (var item in index.Value.Identities)
                {
                    yield return Get(item);
                }
            }
            yield break;
        }

        public Customer Get(CustomerId id)
        {
            Customer customer = null;
            customer = _unitOfWork.Get<Customer>(id);
            if (customer == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                customer = new Customer(stream.Events);
                _unitOfWork.RegisterForTracking(customer, id);
            }
            return customer;
        }

        public Customer Load(CustomerId id)
        {
            Customer customer = null;
            customer = _unitOfWork.Get<Customer>(id);
            if (customer == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                if (stream.Version > 0)
                {
                    customer = new Customer(stream.Events);
                    _unitOfWork.RegisterForTracking(customer, id);
                    return customer;
                }
            }
            return null;
        }
    }
}
