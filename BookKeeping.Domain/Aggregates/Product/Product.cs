using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates.Product
{
    public class Product
    {
        public readonly IList<IEvent> _changes = new List<IEvent>();
        private readonly ProductState _state;

        public Product(IEnumerable<IEvent> events)
        {
            _state = new ProductState(events);
        }

        private void Apply(IEvent e)
        {
            _state.Mutate(e);
            _changes.Add(e);
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
            Apply(new ProductStockUpdated(_state.Id, quantity, reason, utc));
        }

        public void Rename(string title, DateTime utc)
        {
            Apply(new ProductRenamed(_state.Id, title, utc));
        }

        public void ChangeBarcode(Barcode barcode, DateTime utc)
        {
            Apply(new ProductBarcodeChanged(_state.Id, barcode, utc));
        }

        public void ChangeItemNo(string itemNo, DateTime utc)
        {
            Apply(new ProductItemNoChanged(_state.Id, itemNo, utc));
        }

        public void ChangePrice(CurrencyAmount price, DateTime utc)
        {
            Apply(new ProductPriceChanged(_state.Id, price, utc));
        }

        public void ChangeUnitOfMeasure(string unitOfMeasure, DateTime utc)
        {
            Apply(new ProductUnitOfMeasureChanged(_state.Id, unitOfMeasure, utc));
        }

        public void ChangeVatRate(VatRate vatRate, DateTime utc)
        {
            Apply(new ProductVatRateChanged(_state.Id, vatRate, utc));
        }

        public void MakeOrderable(DateTime utc)
        {
            Apply(new ProductMakedOrderable(_state.Id, utc));
        }

        public void MakeNonOrderable(string reason, DateTime utc)
        {
            Apply(new ProductMakedNonOrderable(_state.Id, reason, utc));
        }

        public IList<IEvent> Changes { get { return _changes; } }
    }
}
