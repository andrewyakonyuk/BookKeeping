namespace BookKeeping.Domain.Models
{
    public class OriginalUnitPrice : ICopyable<OriginalUnitPrice>
    {
        public decimal Value
        {
            get;
            set;
        }

        public long CurrencyId
        {
            get;
            set;
        }

        public OriginalUnitPrice(decimal value, long currencyId)
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
            return originalUnitPrice != null && this.Value == originalUnitPrice.Value && this.CurrencyId == originalUnitPrice.CurrencyId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}