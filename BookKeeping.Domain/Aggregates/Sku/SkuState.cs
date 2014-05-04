using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Sku
{
    public class SkuState
    {
        public SkuId Id { get; private set; }

        public WarehouseId Warehouse { get; private set; }

        public string Title { get; private set; }

        public string Barcode { get; private set; }

        public string ItemNo { get; private set; }

        public CurrencyAmount Price { get; private set; }

        public double Stock { get; private set; }

        public string UnitOfMeasure { get; private set; }

        public VatRate VatRate { get; private set; }

        public SkuState(IEnumerable<IEvent> events)
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

        public void When(SkuCreated e)
        {
            Id = e.Id;
            Warehouse = e.Warehouse;
            Title = e.Title;
            ItemNo = e.ItemNo;
            Price = e.Price;
            Stock = e.Stock;
            UnitOfMeasure = e.UnitOfMeasure;
            VatRate = e.VatRate;
        }

        public void When(SkuStockUpdated e)
        {
            Stock = e.Quantity;
        }

        public void When(SkuRenamed e)
        {
            Title = e.NewTitle;
        }

        public void When(SkuBarcodeChanged e)
        {
            Barcode = e.NewBarcode;
        }

        public void When(SkuItemNoChanged e)
        {
            ItemNo = e.NewItemNo;
        }

        public void When(SkuPriceChanged e)
        {
            Price = e.NewPrice;
        }

        public void When(SkuUnitOfMeasureChanged e)
        {
            UnitOfMeasure = e.NewUnitOfMeasure;
        }

        public void When(SkuVatRateChanged e)
        {
            VatRate = e.NewVatRate;
        }

        public void When(SkuMovedToWarehouse e)
        {
            Warehouse = e.DestinationWarehouse;
        }
    }
}
