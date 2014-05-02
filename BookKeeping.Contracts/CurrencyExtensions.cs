using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Contracts
{
    /// <summary>
    /// A simple helper class that allows to express currency as
    /// <code>3m.Eur()</code>
    /// </summary>
    public static class CurrencyExtension
    {
        public static CurrencyAmount Eur(this decimal amount)
        {
            return new CurrencyAmount(amount, Currency.Eur);
        }
    }
}
