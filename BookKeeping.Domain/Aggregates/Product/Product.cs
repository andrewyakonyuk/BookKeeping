using BookKeeping.Core.Domain;
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

        public void Create(ProductId id, string title, string itemNo, CurrencyAmount price, double stock, DateTime utc)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title should be not null or empty", "title");
            if (price.Amount < 0)
                throw new ArgumentException("Price should be prositive", "price");

            Apply(new ProductCreated
            {
                Id = id,
                Title = title,
                ItemNo = itemNo,
                Price = price,
                Stock = stock,
                Created = utc
            });
        }

        public void UpdateStock(double quantity, string reason, DateTime utc)
        {
            Apply(new ProductStockUpdated
            {
                Id = _state.Id,
                Quantity = quantity,
                Reason = reason,
                Updated = utc
            });
        }

        public void Rename(string title, DateTime utc)
        {
            Apply(new ProductRenamed
            {
                Id = _state.Id,
                NewTitle = title,
                Renamed = utc
            });
        }

        public void ChangeBarcode(string barcode, DateTime utc)
        {
            Apply(new ProductBarcodeChanged
            {
                Id = _state.Id,
                NewBarcode = barcode,
                Changed = utc
            });
        }

        public void ChangeItemNo(string itemNo, DateTime utc)
        {
            Apply(new ProductItemNoChanged
            {
                Id = _state.Id,
                NewItemNo = itemNo,
                Changed = utc
            });
        }

        public void ChangePrice(CurrencyAmount price, DateTime utc)
        {
            Apply(new ProductPriceChanged
            {
                Id = _state.Id,
                NewPrice = price,
                Changed = utc
            });
        }

        public void ChangeUOM(string uom, DateTime utc)
        {
            Apply(new ProductUOMChanged
            {
                Id = _state.Id,
                NewUOM = uom,
                Changed = utc
            });
        }

        public void ChangeVAT(double vat, DateTime utc)
        {
            Apply(new ProductVATChanged
            {
                Id = _state.Id,
                NewVAT = vat,
                Changed = utc
            });
        }

        public IList<IEvent> Changes { get { return _changes; } }
    }
}
