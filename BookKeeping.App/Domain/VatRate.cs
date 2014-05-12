using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.App.Domain
{
    public class VatRate
    {
        public virtual Decimal VatPersentage { get; set; }
        static VatRate _zero = new VatRate(0);

        public VatRate()
        {
            this.VatPersentage = new Decimal(0);
        }

        public VatRate(Decimal value)
        {
            this.VatPersentage = value;
        }

        public static VatRate Zero { get { return _zero; } }

        public static Decimal operator +(Decimal value, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value + vatRate.VatPersentage;
        }

        public static Decimal operator +(VatRate vatRate, Decimal value)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value + vatRate.VatPersentage;
        }

        public static Decimal operator *(Decimal value, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value * vatRate.VatPersentage;
        }

        public static Decimal operator *(VatRate vatRate, Decimal value)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value * vatRate.VatPersentage;
        }

        public override string ToString()
        {
            return this.VatPersentage.ToString("p");
        }

        public override bool Equals(object obj)
        {
            VatRate vatRate = obj as VatRate;
            if (vatRate == null)
                return false;
            else
                return this.VatPersentage == vatRate.VatPersentage;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)VatPersentage * 321;
            }
        }
    }
}
