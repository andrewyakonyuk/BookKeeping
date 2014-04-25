using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.Domain.Models
{
    public class OrderLine : ICopyable<OrderLine>
    {
        public long Id
        {
            get;
            set;
        }

        public string Sku
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ProductIdentifier
        {
            get;
            set;
        }

        public decimal Quantity
        {
            get;
            set;
        }

        public long? VatGroupId
        {
            get;
            set;
        }

        public long? LanguageId
        {
            get;
            set;
        }

        public string BundleIdentifier
        {
            get;
            set;
        }

        public long? CopiedFromOrderLineId
        {
            get;
            set;
        }

        public OrderLineCollection OrderLines
        {
            get;
            set;
        }

        public CustomPropertyCollection Properties
        {
            get;
            set;
        }

        public OriginalUnitPriceCollection OriginalUnitPrices
        {
            get;
            set;
        }

        public VatRate VatRate
        {
            get;
            set;
        }

        public PriceCollection UnitPrices
        {
            get;
            set;
        }

        public Price UnitPrice
        {
            get;
            set;
        }

        public PriceCollection TotalPrices
        {
            get;
            set;
        }

        public Price TotalPrice
        {
            get;
            set;
        }

        public PriceCollection BundleUnitPrices
        {
            get;
            set;
        }

        public Price BundleUnitPrice
        {
            get;
            set;
        }

        public PriceCollection BundleTotalPrices
        {
            get;
            set;
        }

        public Price BundleTotalPrice
        {
            get;
            set;
        }

        public OrderLine()
        {
            this.OrderLines = new OrderLineCollection();
            this.Properties = new CustomPropertyCollection();
            this.OriginalUnitPrices = new OriginalUnitPriceCollection();
            this.VatRate = new VatRate();
            this.UnitPrices = new PriceCollection();
            this.UnitPrice = new Price();
            this.TotalPrices = new PriceCollection();
            this.TotalPrice = new Price();
            this.BundleUnitPrices = new PriceCollection();
            this.BundleUnitPrice = new Price();
            this.BundleTotalPrices = new PriceCollection();
            this.BundleTotalPrice = new Price();
        }

        public OrderLine(ProductSnapshot productSnapshot)
            : this()
        {
            Contract.Requires<ArgumentNullException>(productSnapshot != null, "productSnapshot");
            this.Sku = ((!string.IsNullOrEmpty(productSnapshot.Sku)) ? productSnapshot.Sku : productSnapshot.ProductIdentifier);
            this.Name = productSnapshot.Name;
            this.ProductIdentifier = productSnapshot.ProductIdentifier;
            this.VatGroupId = productSnapshot.VatGroupId;
            this.LanguageId = productSnapshot.LanguageId;
            this.Properties = productSnapshot.Properties.Copy();
            this.OriginalUnitPrices = productSnapshot.OriginalUnitPrices.Copy();
        }

        public void ChangeQuantity(decimal quantity)
        {
            this.Quantity = quantity;
        }

        public OrderLine Copy()
        {
            return new OrderLine
            {
                Sku = this.Sku,
                Name = this.Name,
                ProductIdentifier = this.ProductIdentifier,
                Quantity = this.Quantity,
                VatGroupId = this.VatGroupId,
                BundleIdentifier = this.BundleIdentifier,
                CopiedFromOrderLineId = new long?(this.Id),
                OrderLines = this.OrderLines.Copy(),
                Properties = this.Properties.Copy(),
                OriginalUnitPrices = this.OriginalUnitPrices.Copy(),
                VatRate = this.VatRate,
                UnitPrices = this.UnitPrices.Copy(),
                UnitPrice = this.UnitPrice.Copy(),
                TotalPrices = this.TotalPrices.Copy(),
                TotalPrice = this.TotalPrice.Copy(),
                BundleUnitPrices = this.BundleUnitPrices.Copy(),
                BundleUnitPrice = this.BundleUnitPrice.Copy(),
                BundleTotalPrices = this.BundleTotalPrices.Copy(),
                BundleTotalPrice = this.BundleTotalPrice.Copy()
            };
        }

        public override bool Equals(object obj)
        {
            OrderLine orderLine = obj as OrderLine;
            return orderLine != null && (this.Id == orderLine.Id && this.Sku == orderLine.Sku && this.Name == orderLine.Name && this.ProductIdentifier == orderLine.ProductIdentifier && this.Quantity == orderLine.Quantity && this.VatGroupId == orderLine.VatGroupId && this.LanguageId == orderLine.LanguageId && this.BundleIdentifier == orderLine.BundleIdentifier && this.OrderLines.Equals(orderLine.OrderLines) && this.Properties.Equals(orderLine.Properties) && this.OriginalUnitPrices.Equals(orderLine.OriginalUnitPrices) && this.VatRate.Equals(orderLine.VatRate) && this.UnitPrices.Equals(orderLine.UnitPrices) && this.UnitPrice.Equals(orderLine.UnitPrice) && this.TotalPrices.Equals(orderLine.TotalPrices) && this.TotalPrice.Equals(orderLine.TotalPrice) && this.BundleUnitPrices.Equals(orderLine.BundleUnitPrices) && this.BundleUnitPrice.Equals(orderLine.BundleUnitPrice) && this.BundleTotalPrices.Equals(orderLine.BundleTotalPrices)) && this.BundleTotalPrice.Equals(orderLine.BundleTotalPrice);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}