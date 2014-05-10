using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Contracts.Product;
using BookKeeping.Domain.Contracts.Product.Events;
using BookKeeping.Domain.Contracts.Store;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Product
{
    public class ProductState
    {
        public ProductId Id { get; private set; }

        //TODO:
        public StoreId Warehouse { get; private set; }

        public string Title { get; private set; }

        public string Barcode { get; private set; }

        public string ItemNo { get; private set; }

        public CurrencyAmount Price { get; private set; }

        public double Stock { get; private set; }

        public string UnitOfMeasure { get; private set; }

        public VatRate VatRate { get; private set; }

        public ProductState(IEnumerable<IEvent> events)
        {
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
            Warehouse = e.Store;
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
    }
}
