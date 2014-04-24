using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.Domain.Models
{
    public class Price : ICopyable<Price>
    {
        public long CurrencyId { get; private set; }

        public Decimal Value { get; private set; }

        public Decimal Vat { get; private set; }

        public Decimal WithVat { get; private set; }

        public string Formatted { get; private set; }

        public string VatFormatted { get; private set; }

        public string WithVatFormatted { get; private set; }

        public string FormattedWithoutSymbol { get; private set; }

        public string VatFormattedWithoutSymbol { get; private set; }

        public string WithVatFormattedWithoutSymbol { get; private set; }

        public Price()
        {
        }

        public Price(Decimal value, VatRate vatRate, Currency currency)
            : this(value, value * vatRate, value + value * vatRate, currency)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
        }

        public Price(Decimal value, Decimal vat, Decimal withVat, Currency currency)
            : this()
        {
            Contract.Requires<ArgumentNullException>(currency != null, "currency");
            this.Value = value;
            this.Vat = vat;
            this.WithVat = withVat;
            if (currency == null)
                return;
            this.CurrencyId = currency.Id;
            this.Formatted = currency.FormatMoney(this.Value);
            this.VatFormatted = currency.FormatMoney(this.Vat);
            this.WithVatFormatted = currency.FormatMoney(this.WithVat);
            this.FormattedWithoutSymbol = currency.FormatMoneyWithoutSymbol(this.Value);
            this.VatFormattedWithoutSymbol = currency.FormatMoneyWithoutSymbol(this.Vat);
            this.WithVatFormattedWithoutSymbol = currency.FormatMoneyWithoutSymbol(this.WithVat);
        }

        public Price Copy()
        {
            return new Price()
            {
                CurrencyId = this.CurrencyId,
                Value = this.Value,
                Vat = this.Vat,
                WithVat = this.WithVat,
                Formatted = this.Formatted,
                VatFormatted = this.VatFormatted,
                WithVatFormatted = this.WithVatFormatted,
                FormattedWithoutSymbol = this.FormattedWithoutSymbol,
                VatFormattedWithoutSymbol = this.VatFormattedWithoutSymbol,
                WithVatFormattedWithoutSymbol = this.WithVatFormattedWithoutSymbol
            };
        }

        public override string ToString()
        {
            return this.WithVatFormatted;
        }

        public override bool Equals(object obj)
        {
            Price price = obj as Price;
            if (price == null || this.CurrencyId != price.CurrencyId || !(this.Value == price.Value))
                return false;
            else
                return this.WithVat == price.WithVat;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}