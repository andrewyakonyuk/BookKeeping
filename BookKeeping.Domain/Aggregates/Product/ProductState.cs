using BookKeeping.Infrastructure.Domain;
using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Product
{
    public class ProductState
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

        public ProductState(IEnumerable<IEvent> events)
        {
            IsOrderable = true;
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public void Mutate(IEvent e)
        {
            ((dynamic)this).When((dynamic)e);
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
