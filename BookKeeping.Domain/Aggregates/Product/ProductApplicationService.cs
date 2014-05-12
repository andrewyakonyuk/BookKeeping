using BookKeeping.Core;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
using BookKeeping.Domain.Contracts.Product.Commands;
using BookKeeping.Domain.Services.StoreIndex;
using System;

namespace BookKeeping.Domain.Aggregates.Product
{
    public class ProductApplicationService : 
        ICommandHandler<CreateProduct>,
        ICommandHandler<UpdateProductStock>,
        ICommandHandler<RenameProduct>,
        ICommandHandler<ChangeProductBarcode>,
        ICommandHandler<ChangeProductItemNo>,
        ICommandHandler<ChangeProductPrice>,
        ICommandHandler<ChangeProductUnitOfMeasure>,
        ICommandHandler<ChangeProductVatRate>,
        ICommandHandler<MakeProductOrderable>,
        ICommandHandler<MakeProductNonOrderable>
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IStoreIndexService _warehouseService;

        public ProductApplicationService(IEventStore eventStore, IEventBus eventBus, IStoreIndexService warehouseService)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _warehouseService = warehouseService;
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

        public void When(CreateProduct c)
        {
            Update(c.Id, p => p.Create(c.Id, c.Store, c.Title, c.ItemNo, c.Price, c.Stock, c.UnitOfMeasure, c.VatRate, _warehouseService, Current.UtcNow));
        }

        public void When(UpdateProductStock c)
        {
            Update(c.Id, p => p.UpdateStock(c.Quantity, c.Reason, Current.UtcNow));
        }

        public void When(RenameProduct c)
        {
            Update(c.Id, p => p.Rename(c.NewTitle, Current.UtcNow));
        }

        public void When(ChangeProductBarcode c)
        {
            Update(c.Id, p => p.ChangeBarcode(c.NewBarcode, Current.UtcNow));
        }

        public void When(ChangeProductItemNo c)
        {
            Update(c.Id, p => p.ChangeItemNo(c.NewItemNo, Current.UtcNow));
        }

        public void When(ChangeProductPrice c)
        {
            Update(c.Id, p => p.ChangePrice(c.NewPrice, Current.UtcNow));
        }

        public void When(ChangeProductUnitOfMeasure c)
        {
            Update(c.Id, p => p.ChangeUnitOfMeasure(c.NewUnitOfMeasure, Current.UtcNow));
        }

        public void When(ChangeProductVatRate c)
        {
            Update(c.Id, p => p.ChangeVatRate(c.NewVatRate, Current.UtcNow));
        }

        public void When(MakeProductOrderable c)
        {
            Update(c.Id, p => p.MakeOrderable(Current.UtcNow));
        }

        public void When(MakeProductNonOrderable c)
        {
            Update(c.Id, p => p.MakeNonOrderable(c.Reason, Current.UtcNow));
        }
    }
}
