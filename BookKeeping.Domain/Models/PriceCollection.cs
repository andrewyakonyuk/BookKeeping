using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class PriceCollection : List<Price>, ICopyable<PriceCollection>
    {
        public Price Get(long currencyId)
        {
            return this.SingleOrDefault((Price ol) => ol.CurrencyId == currencyId);
        }

        public PriceCollection Copy()
        {
            PriceCollection priceCollection = new PriceCollection();
            priceCollection.AddRange(
                from price in this
                select price.Copy());
            return priceCollection;
        }

        public override bool Equals(object obj)
        {
            PriceCollection priceCollection = obj as PriceCollection;
            if (priceCollection == null)
            {
                return false;
            }
            bool flag = base.Count == priceCollection.Count;
            if (flag)
            {
                flag = !this.Any((Price p) => priceCollection.All((Price cp) => !cp.Equals(p)));
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}