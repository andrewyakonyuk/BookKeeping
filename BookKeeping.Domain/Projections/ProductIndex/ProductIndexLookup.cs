using BookKeeping.Domain.Contracts;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Projections.ProductIndex
{
    [DataContract]
    public sealed class ProductIndexLookup
    {
        [DataMember(Order = 1)]
        public IList<ProductId> Identities { get; private set; }

        public ProductIndexLookup()
        {
            Identities = new List<ProductId>();
        }
    }
}
