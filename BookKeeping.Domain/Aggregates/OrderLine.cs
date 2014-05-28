//using System;
//using System.Diagnostics.Contracts;

//namespace BookKeeping.Domain.Aggregates
//{
//    public class OrderLine : IEntity
//    {
//        public OrderLine()
//        {
//            this.ItemNo = string.Empty;
//            this.Title = string.Empty;
//            this.VatRate = Domain.VatRate.Zero;
//            this.UnitPrice = CurrencyAmount.Unspecifined;
//        }

//        public OrderLine(Product product)
//            : this(product, 1)
//        {
//            Contract.Requires<ArgumentNullException>(product != null, "product");
//        }

//        public OrderLine(Product product, decimal quantity)
//            : this()
//        {
//            Contract.Requires<ArgumentNullException>(product != null, "product");
//            this.ItemNo = product.ItemNo;
//            this.Title = product.Title;
//            this.ProductId = product.Id;
//            this.VatRate = product.VatRate;
//            this.UnitPrice = product.Price;
//            this.Quantity = quantity;
//        }

//        public long Id { get; set; }

//        public long ProductId { get; protected set; }

//        public string ItemNo { get; protected set; }

//        public string Title { get; set; }

//        public Decimal Quantity { get; set; }

//        public VatRate VatRate { get; protected set; }

//        public CurrencyAmount UnitPrice { get; protected set; }

//        public CurrencyAmount UnitPriceInclVat
//        {
//            get
//            {
//                return UnitPrice + UnitPrice * (VatRate.VatPersentage / 100M);
//            }
//        }

//        public CurrencyAmount TotalPrice
//        {
//            get
//            {
//                return UnitPrice * Quantity;
//            }
//        }

//        public CurrencyAmount TotalPriceInclVat
//        {
//            get
//            {
//                return UnitPriceInclVat * Quantity;
//            }
//        }

//        public override bool Equals(object obj)
//        {
//            var orderLine = obj as OrderLine;
//            if (orderLine == null)
//            {
//                return false;
//            }
//            return this.Id == orderLine.Id
//                && this.ItemNo == orderLine.ItemNo
//            && this.ProductId == orderLine.ProductId
//            && this.Quantity == orderLine.Quantity
//            && this.Title == orderLine.Title
//            && this.UnitPrice.Equals(orderLine.UnitPrice)
//            && this.VatRate.Equals(orderLine.VatRate);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                return (int)(this.Id + this.ProductId) * 107;
//            }
//        }
//    }
//}
