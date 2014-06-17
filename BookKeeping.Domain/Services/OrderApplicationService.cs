using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Repositories;
using BookKeeping;
using BookKeeping.Domain;
using BookKeeping.Persistance.Storage;
using System;

namespace BookKeeping.Domain.Services
{
    public class OrderApplicationService : IOrderApplicationService,
        ICommandHandler<CreateOrder>,
        ICommandHandler<CompleteOrder>,
        ICommandHandler<AddProductToOrder>,
        ICommandHandler<UpdateProductQuantityInOrder>,
        ICommandHandler<RemoveProductFromOrder>
    {
        readonly IRepository<Order, OrderId> _repository;

        public OrderApplicationService(IRepository<Order, OrderId> repository)
        {
            _repository = repository;
        }

        private void Update(OrderId id, Action<Order> execute)
        {
            var product = _repository.Get(id);
            execute(product);
        }

        public void When(CreateOrder c)
        {
            Update(c.Id, o => o.Create(c.Id, c.CustomerId, Current.Identity.Id, Current.UtcNow));
        }

        public void When(CompleteOrder c)
        {
            Update(c.Id, o => o.Complete(Current.Identity.Id, Current.UtcNow));
        }

        public void When(AddProductToOrder c)
        {
            Update(c.Id, o => o.AddLine(c.ProductId, c.ItemNo, c.Title, c.Quantity, c.Amount, c.VatRate, Current.Identity.Id, Current.UtcNow));
        }

        public void When(UpdateProductQuantityInOrder c)
        {
            Update(c.Id, o => o.UpdateLineQuantity(c.ProductId, c.Quantity, Current.Identity.Id, Current.UtcNow));
        }

        public void When(RemoveProductFromOrder c)
        {
            Update(c.Id, o => o.RemoveLine(c.ProductId, Current.Identity.Id, Current.UtcNow));
        }
    }
}
