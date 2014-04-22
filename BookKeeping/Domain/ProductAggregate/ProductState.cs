using BookKeeping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.ProductAggregate
{
    public class ProductState
    {
        public ProductId Id { get; private set; }

        public string Title { get; private set; }

        public string Barcode { get; private set; }

        public string ItemNo { get; private set; }

        public CurrencyAmount Price { get; private set; }

        public double Stock { get; private set; }

        public string UOM { get; private set; }

        public double VAT { get; private set; }

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
            Title = e.Title;
            ItemNo = e.ItemNo;
            Price = e.Price;
            Stock = e.Stock;
        }

        public void When(ProductStockUpdated e)
        {
            Stock += e.Quantity;
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

        public void When(ProductUOMChanged e)
        {
            UOM = e.NewUOM;
        }

        public void When(ProductVATChanged e)
        {
            VAT = e.NewVAT;
        }
    }
}
