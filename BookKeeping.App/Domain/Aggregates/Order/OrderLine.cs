using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.App.Domain.Aggregates.Order
{
    public class OrderLine : IEntity
    {
        public OrderLine()
        {
            this.ItemNo = string.Empty;
            this.Title = string.Empty;
            this.VatRate = Domain.VatRate.Zero;
            this.UnitPrice = CurrencyAmount.Unspecifined;
            this.TotalPrice = CurrencyAmount.Unspecifined;
        }

        public OrderLine(Product product)
            : this(product, 1)
        {

        }

        public OrderLine(Product product, decimal quantity)
            : this()
        {
            Contract.Requires<ArgumentNullException>(product != null, "product");
            this.ItemNo = !string.IsNullOrEmpty(product.ItemNo) ? product.ItemNo : product.Id.ToString(); //TODO: format product id
            this.Title = product.Title;
            this.ProductId = product.Id;
            this.VatRate = product.VatRate;
            this.UnitPrice = product.Price;
            this.Quantity = quantity;
            this.TotalPrice = new CurrencyAmount(product.Price.Amount * quantity, product.Price.Currency);
        }

        public long Id { get; set; }

        public long ProductId { get; set; }

        public string ItemNo { get; set; }

        public string Title { get; set; }

        public Decimal Quantity { get; set; }

        public VatRate VatRate { get; set; }

        public CurrencyAmount UnitPrice { get; set; }

        public CurrencyAmount TotalPrice { get; protected set; }

        public void ChangeQuantity(decimal quantity)
        {
            this.Quantity = quantity;
            this.TotalPrice = new CurrencyAmount(UnitPrice.Amount * quantity, UnitPrice.Currency);
        }
    }
}
