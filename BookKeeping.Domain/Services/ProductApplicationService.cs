using System;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent.Storage;
using BookKeeping.Domain.Repositories;

namespace BookKeeping.Domain.Services
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
        readonly IRepository<Product, ProductId> _repository;

        public ProductApplicationService(IRepository<Product, ProductId> repository)
        {
            _repository = repository;
        }

        private void Update(ProductId id, Action<Product> execute)
        {
            var product = _repository.Get(id);
            execute(product);
            _repository.Save(product);
        }

        public void When(CreateProduct c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.Create(c.Id, c.Title, c.ItemNo, c.Price, c.Stock, c.UnitOfMeasure, c.VatRate, c.Barcode, Current.Identity.Id, Current.UtcNow));
        }

        public void When(UpdateProductStock c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.UpdateStock(c.Quantity, c.Reason, Current.Identity.Id, Current.UtcNow));
        }

        public void When(RenameProduct c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.Rename(c.NewTitle, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChangeProductBarcode c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.ChangeBarcode(c.NewBarcode, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChangeProductItemNo c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.ChangeItemNo(c.NewItemNo, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChangeProductPrice c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.ChangePrice(c.NewPrice, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChangeProductUnitOfMeasure c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.ChangeUnitOfMeasure(c.NewUnitOfMeasure, Current.Identity.Id, Current.UtcNow));
        }

        public void When(ChangeProductVatRate c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.ChangeVatRate(c.NewVatRate, Current.Identity.Id, Current.UtcNow));
        }

        public void When(MakeProductOrderable c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.MakeOrderable(Current.Identity.Id, Current.UtcNow));
        }

        public void When(MakeProductNonOrderable c)
        {
            if (Current.Identity == null)
                throw new InvalidOperationException("UserIdentity should be not null");
            Update(c.Id, p => p.MakeNonOrderable(c.Reason, Current.Identity.Id, Current.UtcNow));
        }
    }
}