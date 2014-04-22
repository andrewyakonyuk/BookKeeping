using BookKeeping.Core;
using BookKeeping.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.ProductAggregate
{
    public class ProductApplicationService : 
        ICommandHandler<CreateProduct>,
        ICommandHandler<UpdateProductStock>,
        ICommandHandler<RenameProduct>,
        ICommandHandler<ChangeProductBarcode>,
        ICommandHandler<ChangeProductItemNo>,
        ICommandHandler<ChangeProductPrice>,
        ICommandHandler<ChangeProductUOM>,
        ICommandHandler<ChangeProductVAT>
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

        public void When(CreateProduct c)
        {
            Update(c.Id, p => p.Create(c.Id, c.Title, c.ItemNo, c.Price, c.Stock, DateTime.UtcNow));
        }

        public void When(UpdateProductStock c)
        {
            Update(c.Id, p => p.UpdateStock(c.Quantity, c.Reason, DateTime.UtcNow));
        }

        public void When(RenameProduct c)
        {
            Update(c.Id, p => p.Rename(c.NewTitle, DateTime.UtcNow));
        }

        public void When(ChangeProductBarcode c)
        {
            Update(c.Id, p => p.ChangeBarcode(c.NewBarcode, DateTime.UtcNow));
        }

        public void When(ChangeProductItemNo c)
        {
            Update(c.Id, p => p.ChangeItemNo(c.NewItemNo, DateTime.UtcNow));
        }

        public void When(ChangeProductPrice c)
        {
            Update(c.Id, p => p.ChangePrice(c.NewPrice, DateTime.UtcNow));
        }

        public void When(ChangeProductUOM c)
        {
            Update(c.Id, p => p.ChangeUOM(c.NewUOM, DateTime.UtcNow));
        }

        public void When(ChangeProductVAT c)
        {
            Update(c.Id, p => p.ChangeVAT(c.NewVAT, DateTime.UtcNow));
        }
    }
}
