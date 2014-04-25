using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Models
{
    public class OriginalUnitPriceCollection : List<OriginalUnitPrice>, ICopyable<OriginalUnitPriceCollection>
    {
        public OriginalUnitPriceCollection Copy()
        {
            OriginalUnitPriceCollection originalUnitPriceCollection = new OriginalUnitPriceCollection();
            originalUnitPriceCollection.AddRange(
                from originalUnitPrice in this
                select originalUnitPrice.Copy());
            return originalUnitPriceCollection;
        }

        public override bool Equals(object obj)
        {
            OriginalUnitPriceCollection originalUnitPriceCollection = obj as OriginalUnitPriceCollection;
            if (originalUnitPriceCollection == null)
            {
                return false;
            }
            bool flag = base.Count == originalUnitPriceCollection.Count;
            if (flag)
            {
                flag = !this.Any((OriginalUnitPrice p) => originalUnitPriceCollection.All((OriginalUnitPrice cp) => !cp.Equals(p)));
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}