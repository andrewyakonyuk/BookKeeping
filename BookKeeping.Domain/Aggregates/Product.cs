using System;
using System.Collections.Generic;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain.Aggregates
{
    public class Product : AggregateBase, IProductState
    {
        public ProductId Id { get; private set; }

        public string Title { get; private set; }

        public Barcode Barcode { get; private set; }

        public string ItemNo { get; private set; }

        public CurrencyAmount Price { get; private set; }

        public decimal Stock { get; private set; }

        public string UnitOfMeasure { get; private set; }

        public VatRate VatRate { get; private set; }

        public bool IsOrderable { get; private set; }

        public Product(IEnumerable<IEvent> events)
            : base(events)
        {
            IsOrderable = true;
        }

        public void Create(ProductId id, string title, string itemNo, CurrencyAmount price,
            decimal stock, string unitOfMeasure, VatRate vatRate, Barcode barcode, UserId userId, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title should be not null or empty", "title");
            if (price.Amount < 0)
                throw new ArgumentException("Price should be prositive", "price");

            Apply(new ProductCreated(id, title, itemNo, price, stock, unitOfMeasure, vatRate, barcode, userId, utc));
        }

        public void UpdateStock(decimal quantity, string reason, UserId userId, DateTime utc)
        {
            Apply(new ProductStockUpdated(this.Id, quantity, reason, userId, utc));
        }

        public void Rename(string title, UserId userId, DateTime utc)
        {
            Apply(new ProductRenamed(this.Id, title, userId, utc));
        }

        public void ChangeBarcode(Barcode barcode, UserId userId, DateTime utc)
        {
            Apply(new ProductBarcodeChanged(this.Id, barcode, userId, utc));
        }

        public void ChangeItemNo(string itemNo, UserId userId, DateTime utc)
        {
            Apply(new ProductItemNoChanged(this.Id, itemNo, userId, utc));
        }

        public void ChangePrice(CurrencyAmount price, UserId userId, DateTime utc)
        {
            Apply(new ProductPriceChanged(this.Id, price, userId, utc));
        }

        public void ChangeUnitOfMeasure(string unitOfMeasure, UserId userId, DateTime utc)
        {
            Apply(new ProductUnitOfMeasureChanged(this.Id, unitOfMeasure, userId, utc));
        }

        public void ChangeVatRate(VatRate vatRate, UserId userId, DateTime utc)
        {
            Apply(new ProductVatRateChanged(this.Id, vatRate, userId, utc));
        }

        public void MakeOrderable(UserId userId, DateTime utc)
        {
            Apply(new ProductMakedOrderable(this.Id, userId, utc));
        }

        public void MakeNonOrderable(string reason, UserId userId, DateTime utc)
        {
            Apply(new ProductMakedNonOrderable(this.Id, reason, userId, utc));
        }

        protected override void Mutate(IEvent e)
        {
            Version += 1;
            ((IProductState)this).When((dynamic)e);
        }

        void IProductState.When(ProductCreated e)
        {
            Id = e.Id;
            Title = e.Title;
            ItemNo = e.ItemNo;
            Price = e.Price;
            Stock = e.Stock;
            UnitOfMeasure = e.UnitOfMeasure;
            VatRate = e.VatRate;
        }

        void IProductState.When(ProductStockUpdated e)
        {
            Stock = e.Quantity;
        }

        void IProductState.When(ProductRenamed e)
        {
            Title = e.NewTitle;
        }

        void IProductState.When(ProductBarcodeChanged e)
        {
            Barcode = e.NewBarcode;
        }

        void IProductState.When(ProductItemNoChanged e)
        {
            ItemNo = e.NewItemNo;
        }

        void IProductState.When(ProductPriceChanged e)
        {
            Price = e.NewPrice;
        }

        void IProductState.When(ProductUnitOfMeasureChanged e)
        {
            UnitOfMeasure = e.NewUnitOfMeasure;
        }

        void IProductState.When(ProductVatRateChanged e)
        {
            VatRate = e.NewVatRate;
        }

        void IProductState.When(ProductMakedOrderable e)
        {
            IsOrderable = true;
        }

        void IProductState.When(ProductMakedNonOrderable e)
        {
            IsOrderable = false;
        }
    }
}