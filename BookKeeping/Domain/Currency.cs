using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain
{
    /// <summary>  Just a currency enumeration, which is
    /// a part of <see cref="CurrencyAmount"/>  </summary>
    public enum Currency
    {
        None,
        Eur,
        Usd,
        Rur
    }
}
