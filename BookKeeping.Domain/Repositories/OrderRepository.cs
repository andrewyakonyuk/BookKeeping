using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.OrderIndex;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public class OrderRepository : IRepository<Order, OrderId>
    {
        private readonly IEventStore _eventStore;
        private readonly IDocumentReader<unit, OrderIndexLookup> _customerIndexReader;
        private readonly IUnitOfWork _unitOfWork;

        public OrderRepository(IEventStore eventStore, IUnitOfWork unitOfWork, IDocumentReader<unit, OrderIndexLookup> customerIndexReader)
        {
            _eventStore = eventStore;
            _customerIndexReader = customerIndexReader;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Order> All()
        {
            var index = _customerIndexReader.Get<OrderIndexLookup>();
            if (index.HasValue)
            {
                foreach (var item in index.Value.Identities)
                {
                    yield return Get(item);
                }
            }
            yield break;
        }

        public Order Get(OrderId id)
        {
            Order customer = null;
            customer = _unitOfWork.Get<Order>(id);
            if (customer == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                customer = new Order(stream.Events);
                _unitOfWork.RegisterForTracking(customer, id);
            }
            return customer;
        }

        public Order Load(OrderId id)
        {
            Order customer = null;
            customer = _unitOfWork.Get<Order>(id);
            if (customer == null)
            {
                var stream = _eventStore.LoadEventStream(id);
                if (stream.Version > 0)
                {
                    customer = new Order(stream.Events);
                    _unitOfWork.RegisterForTracking(customer, id);
                    return customer;
                }
            }
            return null;
        }
    }
}