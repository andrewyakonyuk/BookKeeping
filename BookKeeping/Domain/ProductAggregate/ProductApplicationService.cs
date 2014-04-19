using BookKeeping.Core;
using BookKeeping.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.ProductAggregate
{
    public class ProductApplicationService
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;

        public ProductApplicationService(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        private void Update(ProductId id, Action<Product> execute)
        {
            var stream = _eventStore.LoadEventStream(id);
            var customer = new Product(stream.Events);
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
