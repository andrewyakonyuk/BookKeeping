using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent.Storage;
using System;

namespace BookKeeping.Domain.Services
{
    public class OrderApplicationService
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;

        public OrderApplicationService(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        private void Update(OrderId id, Action<Order> execute)
        {
            var stream = _eventStore.LoadEventStream(id);
            var customer = new Order(stream.Events);
            execute(customer);
            _eventStore.AppendToStream(id, stream.Version, customer.Changes);

            foreach (var @event in customer.Changes)
            {
                var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                _eventBus.Publish(realEvent);
            }
        }
    }
}
