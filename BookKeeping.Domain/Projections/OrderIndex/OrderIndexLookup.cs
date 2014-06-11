using BookKeeping.Domain.Contracts;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Projections.OrderIndex
{
    [DataContract]
    public sealed class OrderIndexLookup
    {
        [DataMember(Order = 1)]
        public IList<OrderId> Identities { get; private set; }

        public OrderIndexLookup()
        {
            Identities = new List<OrderId>();
        }
    }
}
