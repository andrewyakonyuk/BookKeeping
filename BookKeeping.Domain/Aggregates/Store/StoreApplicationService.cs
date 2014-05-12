using BookKeeping.Core;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Store;
using BookKeeping.Domain.Contracts.Store.Commands;
using BookKeeping.Domain.Services.StoreIndex;
using System;

namespace BookKeeping.Domain.Aggregates.Store
{
    public sealed class StoreApplicationService :
        ICommandHandler<CreateStore>,
        ICommandHandler<RenameStore>,
        ICommandHandler<CloseStore>
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IStoreIndexService _warehouseService;

        public StoreApplicationService(IEventStore eventStore, IEventBus eventBus, IStoreIndexService warehouseService)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _warehouseService = warehouseService;
        }

        private void Update(StoreId id, Action<Store> execute)
        {
            var stream = _eventStore.LoadEventStream(id);
            var customer = new Store(stream.Events);
            execute(customer);
            _eventStore.AppendToStream(id, stream.Version, customer.Changes);

            foreach (var @event in customer.Changes)
            {
                var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                _eventBus.Publish(realEvent);
            }
        }

        public void When(CreateStore c)
        {
            Update(c.Id, w => w.Create(c.Id, c.Name, Current.UtcNow));
        }

        public void When(RenameStore c)
        {
            Update(c.Id, w => w.Rename(c.NewName, Current.UtcNow));
        }

        public void When(CloseStore c)
        {
            Update(c.Id, w => w.Close(c.Reason, _warehouseService, Current.UtcNow));
        }
    }
}
