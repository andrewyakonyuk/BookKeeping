using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BookKeeping.Domain.Contracts
{
    /// <summary>
    /// This is an extremely important concept for accounting - amount of money
    /// in a specific currency. It helps to ensure that we will never try to add
    /// amounts in different currencies. </summary>
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public struct CurrencyAmount
    {
        [DataMember(Order = 1)] public readonly decimal Amount;
        [DataMember(Order = 2)] public readonly Currency Currency;

        public CurrencyAmount(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public override bool Equals(object obj)
        {
            if (obj is CurrencyAmount)
            {
                var other = (CurrencyAmount)obj;
                return Currency == other.Currency && this.Amount == other.Amount;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)Amount * 356 + Currency.GetHashCode();
            }
        }

        public static bool operator ==(CurrencyAmount left, CurrencyAmount right)
        {
            left.CheckCurrency(right.Currency, "==");
            return left.Amount == right.Amount;
        }

        public static bool operator !=(CurrencyAmount left, CurrencyAmount right)
        {
            left.CheckCurrency(right.Currency, "!=");
            return left.Amount != right.Amount;
        }

        public static bool operator <(CurrencyAmount left, CurrencyAmount right)
        {
            left.CheckCurrency(right.Currency, "<");
            return left.Amount < right.Amount;
        }

        public static CurrencyAmount operator +(CurrencyAmount left, CurrencyAmount right)
        {
            left.CheckCurrency(right.Currency, "+");
            return new CurrencyAmount(left.Amount + right.Amount, left.Currency);
        }

        public static CurrencyAmount operator -(CurrencyAmount left, CurrencyAmount right)
        {
            left.CheckCurrency(right.Currency, "-");
            return new CurrencyAmount(left.Amount - right.Amount, left.Currency);
        }

        public static CurrencyAmount operator -(CurrencyAmount right)
        {
            return new CurrencyAmount(-right.Amount, right.Currency);
        }

        private void CheckCurrency(Currency type, string operation)
        {
            if (Currency == type) return;
            throw new InvalidOperationException(string.Format("Can't perform operation on different currencies: {0} {1} {2}", Currency, operation, type));
        }

        public static bool operator >(CurrencyAmount left, CurrencyAmount right)
        {
            left.CheckCurrency(right.Currency, ">");
            return left.Amount > right.Amount;
        }

        public override string ToString()
        {
            return string.Format("{0:0.##} {1}", Amount, Currency.ToString().ToUpper());
        }
    }
}
