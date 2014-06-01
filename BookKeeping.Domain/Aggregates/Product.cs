using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;

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
            decimal stock, string unitOfMeasure, VatRate vatRate, Barcode barcode, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title should be not null or empty", "title");
            if (price.Amount < 0)
                throw new ArgumentException("Price should be prositive", "price");

            Apply(new ProductCreated(id, title, itemNo, price, stock, unitOfMeasure, vatRate, barcode, utc));
        }

        public void UpdateStock(decimal quantity, string reason, DateTime utc)
        {
            Apply(new ProductStockUpdated(this.Id, quantity, reason, utc));
        }

        public void Rename(string title, DateTime utc)
        {
            Apply(new ProductRenamed(this.Id, title, utc));
        }

        public void ChangeBarcode(Barcode barcode, DateTime utc)
        {
            Apply(new ProductBarcodeChanged(this.Id, barcode, utc));
        }

        public void ChangeItemNo(string itemNo, DateTime utc)
        {
            Apply(new ProductItemNoChanged(this.Id, itemNo, utc));
        }

        public void ChangePrice(CurrencyAmount price, DateTime utc)
        {
            Apply(new ProductPriceChanged(this.Id, price, utc));
        }

        public void ChangeUnitOfMeasure(string unitOfMeasure, DateTime utc)
        {
            Apply(new ProductUnitOfMeasureChanged(this.Id, unitOfMeasure, utc));
        }

        public void ChangeVatRate(VatRate vatRate, DateTime utc)
        {
            Apply(new ProductVatRateChanged(this.Id, vatRate, utc));
        }

        public void MakeOrderable(DateTime utc)
        {
            Apply(new ProductMakedOrderable(this.Id, utc));
        }

        public void MakeNonOrderable(string reason, DateTime utc)
        {
            Apply(new ProductMakedNonOrderable(this.Id, reason, utc));
        }

        public void When(ProductCreated e)
        {
            Id = e.Id;
            Title = e.Title;
            ItemNo = e.ItemNo;
            Price = e.Price;
            Stock = e.Stock;
            UnitOfMeasure = e.UnitOfMeasure;
            VatRate = e.VatRate;
        }

        public void When(ProductStockUpdated e)
        {
            Stock = e.Quantity;
        }

        public void When(ProductRenamed e)
        {
            Title = e.NewTitle;
        }

        public void When(ProductBarcodeChanged e)
        {
            Barcode = e.NewBarcode;
        }

        public void When(ProductItemNoChanged e)
        {
            ItemNo = e.NewItemNo;
        }

        public void When(ProductPriceChanged e)
        {
            Price = e.NewPrice;
        }

        public void When(ProductUnitOfMeasureChanged e)
        {
            UnitOfMeasure = e.NewUnitOfMeasure;
        }

        public void When(ProductVatRateChanged e)
        {
            VatRate = e.NewVatRate;
        }

        public void When(ProductMakedOrderable e)
        {
            IsOrderable = true;
        }

        public void When(ProductMakedNonOrderable e)
        {
            IsOrderable = false;
        }
    }
}
