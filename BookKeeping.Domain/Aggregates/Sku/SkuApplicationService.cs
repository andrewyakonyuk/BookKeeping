using BookKeeping.Core;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Services.WarehouseIndex;
using System;

namespace BookKeeping.Domain.Aggregates.Sku
{
    public class SkuApplicationService : 
        ICommandHandler<CreateSku>,
        ICommandHandler<UpdateSkuStock>,
        ICommandHandler<RenameSku>,
        ICommandHandler<ChangeSkuBarcode>,
        ICommandHandler<ChangeSkuItemNo>,
        ICommandHandler<ChangeSkuPrice>,
        ICommandHandler<ChangeSkuUnitOfMeasure>,
        ICommandHandler<ChangeSkuVatRate>,
        ICommandHandler<MoveSkuToWarehouse>
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IWarehouseIndexService _warehouseService;

        public SkuApplicationService(IEventStore eventStore, IEventBus eventBus, IWarehouseIndexService warehouseService)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _warehouseService = warehouseService;
        }

        private void Update(SkuId id, Action<Sku> execute)
        {
            var stream = _eventStore.LoadEventStream(id);
            var customer = new Sku(stream.Events);
            execute(customer);
            _eventStore.AppendToStream(id, stream.Version, customer.Changes);

            foreach (var @event in customer.Changes)
            {
                var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                _eventBus.Publish(realEvent);
            }
            _eventBus.Commit();
        }

        public void When(CreateSku c)
        {
            Update(c.Id, p => p.Create(c.Id, c.Warehouse, c.Title, c.ItemNo, c.Price, c.Stock, c.UnitOfMeasure, c.VatRate, _warehouseService, Current.UtcNow));
        }

        public void When(UpdateSkuStock c)
        {
            Update(c.Id, p => p.UpdateStock(c.Quantity, c.Reason, Current.UtcNow));
        }

        public void When(RenameSku c)
        {
            Update(c.Id, p => p.Rename(c.NewTitle, Current.UtcNow));
        }

        public void When(ChangeSkuBarcode c)
        {
            Update(c.Id, p => p.ChangeBarcode(c.NewBarcode, Current.UtcNow));
        }

        public void When(ChangeSkuItemNo c)
        {
            Update(c.Id, p => p.ChangeItemNo(c.NewItemNo, Current.UtcNow));
        }

        public void When(ChangeSkuPrice c)
        {
            Update(c.Id, p => p.ChangePrice(c.NewPrice, Current.UtcNow));
        }

        public void When(ChangeSkuUnitOfMeasure c)
        {
            Update(c.Id, p => p.ChangeUnitOfMeasure(c.NewUnitOfMeasure, Current.UtcNow));
        }

        public void When(ChangeSkuVatRate c)
        {
            Update(c.Id, p => p.ChangeVatRate(c.NewVatRate, Current.UtcNow));
        }

        public void When(MoveSkuToWarehouse c)
        {
            Update(c.Id, p => p.MoveToWarehouse(c.DestinationWarehouse, c.Reason, Current.UtcNow));
        }
    }
}
