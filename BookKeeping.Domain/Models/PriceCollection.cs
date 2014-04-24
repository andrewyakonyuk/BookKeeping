using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class PriceCollection : List<Price>, ICopyable<PriceCollection>
    {
        public Price Get(long currencyId)
        {
            return Enumerable.SingleOrDefault<Price>((IEnumerable<Price>)this, (Func<Price, bool>)(ol => ol.CurrencyId == currencyId));
        }

        public PriceCollection Copy()
        {
            PriceCollection priceCollection = new PriceCollection();
            priceCollection.AddRange(Enumerable.Select<Price, Price>((IEnumerable<Price>)this, (Func<Price, Price>)(price => price.Copy())));
            return priceCollection;
        }

        public override bool Equals(object obj)
        {
            PriceCollection priceCollection = obj as PriceCollection;
            if (priceCollection == null)
                return false;
            bool flag = this.Count == priceCollection.Count;
            if (flag)
                flag = !Enumerable.Any<Price>((IEnumerable<Price>)this, (Func<Price, bool>)(p => Enumerable.All<Price>((IEnumerable<Price>)priceCollection, (Func<Price, bool>)(cp => !cp.Equals((object)p)))));
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}