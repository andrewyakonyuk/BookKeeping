using System;
using System.Diagnostics.Contracts;

namespace BookKeeping.Domain.Contracts
{
    public struct VatRate
    {
        public readonly Decimal VatPersentage;
        static VatRate _zero = new VatRate(0);

        public VatRate(Decimal value)
        {
            VatPersentage = value;
        }

        public static VatRate Zero { get { return _zero; } }

        public static Decimal operator +(Decimal value, VatRate vatRate)
        {
            return value + vatRate.VatPersentage;
        }

        public static Decimal operator +(VatRate vatRate, Decimal value)
        {
            return value + vatRate.VatPersentage;
        }

        public static Decimal operator *(Decimal value, VatRate vatRate)
        {
            return value * vatRate.VatPersentage;
        }

        public static Decimal operator *(VatRate vatRate, Decimal value)
        {
            return value * vatRate.VatPersentage;
        }

        public override string ToString()
        {
            return this.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public string ToString(System.Globalization.CultureInfo culture)
        {
            return this.VatPersentage.ToString("p", culture);
        }

        public override bool Equals(object obj)
        {
            if (obj is VatRate)
            {
                var vatRate = (VatRate)obj;
                return this.VatPersentage == vatRate.VatPersentage;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)VatPersentage * 321;
            }
        }

        public static bool operator ==(VatRate left, VatRate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VatRate left, VatRate right)
        {
            return !left.Equals(right);
        }

        public static bool operator >(VatRate left, VatRate right)
        {
            return left.VatPersentage > right.VatPersentage;
        }

        public static bool operator >(VatRate left, decimal right)
        {
            return left.VatPersentage > right;
        }

        public static bool operator >=(VatRate left, VatRate right)
        {
            return left.VatPersentage >= right.VatPersentage;
        }

        public static bool operator >=(VatRate left, decimal right)
        {
            return left.VatPersentage >= right;
        }

        public static bool operator <(VatRate left, VatRate right)
        {
            return left.VatPersentage < right.VatPersentage;
        }

        public static bool operator <(VatRate left, decimal right)
        {
            return left.VatPersentage < right;
        }

        public static bool operator <=(VatRate left, VatRate right)
        {
            return left.VatPersentage <= right.VatPersentage;
        }

        public static bool operator <=(VatRate left, decimal right)
        {
            return left.VatPersentage <= right;
        }
    }
}
