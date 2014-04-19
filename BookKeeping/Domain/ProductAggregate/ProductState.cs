using BookKeeping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.ProductAggregate
{
    public class ProductState
    {
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

        public ProductId Id { get; private set; }

        public string Title { get; private set; }

        public string Barcode { get; private set; }

        public string ItemNo { get; private set; }

        public CurrencyAmount Price { get; private set; }

        public double Stock { get; private set; }

        public string UOM { get; private set; }

        public double VAT { get; private set; }
    }
}
