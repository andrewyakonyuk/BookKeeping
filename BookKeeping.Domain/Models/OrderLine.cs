using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.Domain.Models
{
    public class OrderLine : ICopyable<OrderLine>
    {
        public long Id { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public string ProductIdentifier { get; set; }

        public Decimal Quantity { get; set; }

        public long? VatGroupId { get; set; }

        public long? LanguageId { get; set; }

        public string BundleIdentifier { get; set; }

        public long? CopiedFromOrderLineId { get; set; }

        public OrderLineCollection OrderLines { get; set; }

        public CustomPropertyCollection Properties { get; set; }

        public OriginalUnitPriceCollection OriginalUnitPrices { get; set; }

        public VatRate VatRate { get; set; }

        public PriceCollection UnitPrices { get; set; }

        public Price UnitPrice { get; set; }

        public PriceCollection TotalPrices { get; set; }

        public Price TotalPrice { get; set; }

        public PriceCollection BundleUnitPrices { get; set; }

        public Price BundleUnitPrice { get; set; }

        public PriceCollection BundleTotalPrices { get; set; }

        public Price BundleTotalPrice { get; set; }

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
            this.Sku = !string.IsNullOrEmpty(productSnapshot.Sku) ? productSnapshot.Sku : productSnapshot.ProductIdentifier;
            this.Name = productSnapshot.Name;
            this.ProductIdentifier = productSnapshot.ProductIdentifier;
            this.VatGroupId = productSnapshot.VatGroupId;
            this.LanguageId = productSnapshot.LanguageId;
            this.Properties = productSnapshot.Properties.Copy();
            this.OriginalUnitPrices = productSnapshot.OriginalUnitPrices.Copy();
        }

        public void ChangeQuantity(Decimal quantity)
        {
            this.Quantity = quantity;
        }

        public OrderLine Copy()
        {
            return new OrderLine()
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
            if (orderLine == null || this.Id != orderLine.Id || (!(this.Sku == orderLine.Sku) || !(this.Name == orderLine.Name)) || (!(this.ProductIdentifier == orderLine.ProductIdentifier) || !(this.Quantity == orderLine.Quantity)))
                return false;
            long? vatGroupId1 = this.VatGroupId;
            long? vatGroupId2 = orderLine.VatGroupId;
            if ((vatGroupId1.GetValueOrDefault() != vatGroupId2.GetValueOrDefault() ? 0 : (vatGroupId1.HasValue == vatGroupId2.HasValue ? 1 : 0)) != 0)
            {
                long? languageId1 = this.LanguageId;
                long? languageId2 = orderLine.LanguageId;
                if ((languageId1.GetValueOrDefault() != languageId2.GetValueOrDefault() ? 0 : (languageId1.HasValue == languageId2.HasValue ? 1 : 0)) != 0 && this.BundleIdentifier == orderLine.BundleIdentifier && (this.OrderLines.Equals((object)orderLine.OrderLines) && this.Properties.Equals((object)orderLine.Properties)) && (this.OriginalUnitPrices.Equals((object)orderLine.OriginalUnitPrices) && this.VatRate.Equals((object)orderLine.VatRate) && (this.UnitPrices.Equals((object)orderLine.UnitPrices) && this.UnitPrice.Equals((object)orderLine.UnitPrice))) && (this.TotalPrices.Equals((object)orderLine.TotalPrices) && this.TotalPrice.Equals((object)orderLine.TotalPrice) && (this.BundleUnitPrices.Equals((object)orderLine.BundleUnitPrices) && this.BundleUnitPrice.Equals((object)orderLine.BundleUnitPrice)) && this.BundleTotalPrices.Equals((object)orderLine.BundleTotalPrices)))
                    return this.BundleTotalPrice.Equals((object)orderLine.BundleTotalPrice);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}