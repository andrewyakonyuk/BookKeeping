using BookKeeping.Domain.Contracts;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Projections.CustomerIndex
{
    [DataContract]
    public sealed class CustomerIndexLookup
    {
        [DataMember(Order = 1)]
        public IList<CustomerId> Identities { get; private set; }

        public CustomerIndexLookup()
        {
            Identities = new List<CustomerId>();
        }
    }
}
