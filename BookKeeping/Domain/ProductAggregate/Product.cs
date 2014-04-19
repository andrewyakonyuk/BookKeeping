using BookKeeping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.ProductAggregate
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

        public void Create(ProductId id, string title, string barcode, string itemNo, CurrencyAmount price,
            double stock, string uom, string manufacturer, double vat, DateTime utc)
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
                Barcode = barcode,
                ItemNo = itemNo,
                Price = price,
                Stock = stock,
                UOM = uom,
                Manufacturer = manufacturer,
                VAT = vat,
                Created = utc
            });
        }

        public IList<IEvent> Changes { get { return _changes; } }
    }
}
