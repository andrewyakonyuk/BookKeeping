using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Aggregates
{
    public class Product : IEntity
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public Barcode Barcode { get; set; }

        public string ItemNo { get; set; }

        public CurrencyAmount Price { get; set; }

        public decimal Stock { get; set; }

        public string UnitOfMeasure { get; set; }

        public VatRate VatRate { get; set; }

        public bool IsOrderable { get; set; }
    }
}
