using System;

namespace BookKeeping.Domain.Models
{
    public class OriginalUnitPrice : ICopyable<OriginalUnitPrice>
    {
        public Decimal Value { get; set; }

        public long CurrencyId { get; set; }

        public OriginalUnitPrice(Decimal value, long currencyId)
        {
            this.Value = value;
            this.CurrencyId = currencyId;
        }

        public OriginalUnitPrice Copy()
        {
            return new OriginalUnitPrice(this.Value, this.CurrencyId);
        }

        public override bool Equals(object obj)
        {
            OriginalUnitPrice originalUnitPrice = obj as OriginalUnitPrice;
            if (originalUnitPrice == null || !(this.Value == originalUnitPrice.Value))
                return false;
            else
                return this.CurrencyId == originalUnitPrice.CurrencyId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}