using BookKeeping.Core;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services.WarehouseIndex;
using System;

namespace BookKeeping.Domain.Aggregates.Warehouse
{
    public sealed class WarehouseApplicationService :
        ICommandHandler<CreateWarehouse>,
        ICommandHandler<RenameWarehouse>,
        ICommandHandler<CloseWarehouse>
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IWarehouseIndexService _warehouseService;

        public WarehouseApplicationService(IEventStore eventStore, IEventBus eventBus, IWarehouseIndexService warehouseService)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _warehouseService = warehouseService;
        }

        private void Update(WarehouseId id, Action<Warehouse> execute)
        {
            var stream = _eventStore.LoadEventStream(id);
            var customer = new Warehouse(stream.Events);
            execute(customer);
            _eventStore.AppendToStream(id, stream.Version, customer.Changes);

            foreach (var @event in customer.Changes)
            {
                var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                _eventBus.Publish(realEvent);
            }
            _eventBus.Commit();
        }

        public void When(CreateWarehouse c)
        {
            Update(c.Id, w => w.Create(c.Id, c.Name, Current.UtcNow));
        }

        public void When(RenameWarehouse c)
        {
            Update(c.Id, w => w.Rename(c.NewName, Current.UtcNow));
        }

        public void When(CloseWarehouse c)
        {
            Update(c.Id, w => w.Close(c.Reason, _warehouseService, Current.UtcNow));
        }
    }
}
