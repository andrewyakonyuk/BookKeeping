using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.Domain.Models
{
    public class PriceWithoutVat : ICopyable<PriceWithoutVat>
    {
        public long CurrencyId { get; private set; }

        public Decimal Value { get; private set; }

        public string Formatted { get; private set; }

        public string FormattedWithoutSymbol { get; private set; }

        public PriceWithoutVat()
        {
        }

        public PriceWithoutVat(Decimal value, Currency currency)
            : this()
        {
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            this.Value = value;
            if (currency == null)
                return;
            this.CurrencyId = currency.Id;
            this.Formatted = currency.FormatMoney(this.Value);
            this.FormattedWithoutSymbol = currency.FormatMoneyWithoutSymbol(this.Value);
        }

        public PriceWithoutVat Copy()
        {
            return new PriceWithoutVat()
            {
                CurrencyId = this.CurrencyId,
                Value = this.Value,
                Formatted = this.Formatted,
                FormattedWithoutSymbol = this.FormattedWithoutSymbol
            };
        }

        public override string ToString()
        {
            return this.Formatted;
        }

        public override bool Equals(object obj)
        {
            PriceWithoutVat priceWithoutVat = obj as PriceWithoutVat;
            if (priceWithoutVat == null || this.CurrencyId != priceWithoutVat.CurrencyId)
                return false;
            else
                return this.Value == priceWithoutVat.Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}