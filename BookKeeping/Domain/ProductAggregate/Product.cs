using System;

namespace BookKeeping.Domain.ProductAggregate
{
    public class Product
    {
        public ProductId Id { get; protected set; }

        public string Title { get; set; }

        public string Barcode { get; set; }

        public string ItemNo { get; set; }

        public CurrencyAmount Price { get; set; }

        public double Stock { get; protected set; }

        public string UOM { get; set; }

        public double VAT { get; set; }

        [Obsolete("Only for NHibernate", true)]
        protected Product()
        {

        }

        public Product(ProductId id, string title, string itemNo, CurrencyAmount price, double stock)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title should be not null or empty", "title");
            if (price.Amount < 0)
                throw new ArgumentException("Price should be prositive", "price");

            Id = id;
            Title = title;
            ItemNo = itemNo;
            Price = price;
            Stock = stock;
        }

        public void UpdateStock(double quantity, string reason)
        {
            Stock += quantity;
        }
    }
}
