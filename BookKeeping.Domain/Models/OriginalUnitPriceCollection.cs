using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class OriginalUnitPriceCollection : List<OriginalUnitPrice>, ICopyable<OriginalUnitPriceCollection>
    {
        public OriginalUnitPriceCollection Copy()
        {
            OriginalUnitPriceCollection unitPriceCollection = new OriginalUnitPriceCollection();
            unitPriceCollection.AddRange(Enumerable.Select<OriginalUnitPrice, OriginalUnitPrice>((IEnumerable<OriginalUnitPrice>)this, (Func<OriginalUnitPrice, OriginalUnitPrice>)(originalUnitPrice => originalUnitPrice.Copy())));
            return unitPriceCollection;
        }

        public override bool Equals(object obj)
        {
            OriginalUnitPriceCollection originalUnitPriceCollection = obj as OriginalUnitPriceCollection;
            if (originalUnitPriceCollection == null)
                return false;
            bool flag = this.Count == originalUnitPriceCollection.Count;
            if (flag)
                flag = !Enumerable.Any<OriginalUnitPrice>((IEnumerable<OriginalUnitPrice>)this, (Func<OriginalUnitPrice, bool>)(p => Enumerable.All<OriginalUnitPrice>((IEnumerable<OriginalUnitPrice>)originalUnitPriceCollection, (Func<OriginalUnitPrice, bool>)(cp => !cp.Equals((object)p)))));
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}