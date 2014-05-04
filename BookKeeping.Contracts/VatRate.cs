using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public class VatRate
    {
        [DataMember(Order = 1)]
        public virtual Decimal Value { get; set; }

        public VatRate()
        {
            this.Value = new Decimal(0);
        }

        public VatRate(Decimal value)
        {
            this.Value = value;
        }

        public static Decimal operator +(Decimal value, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value + vatRate.Value;
        }

        public static Decimal operator +(VatRate vatRate, Decimal value)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value + vatRate.Value;
        }

        public static Decimal operator *(Decimal value, VatRate vatRate)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value * vatRate.Value;
        }

        public static Decimal operator *(VatRate vatRate, Decimal value)
        {
            Contract.Requires<ArgumentNullException>(vatRate != null, "vatRate");
            return value * vatRate.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString("p");
        }

        public override bool Equals(object obj)
        {
            VatRate vatRate = obj as VatRate;
            if (vatRate == null)
                return false;
            else
                return this.Value == vatRate.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)Value * 321;
            }
        }
    }
}
