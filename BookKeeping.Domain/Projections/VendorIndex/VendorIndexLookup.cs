using BookKeeping.Domain.Contracts;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Projections.VendorIndex
{
    [DataContract]
    public sealed class VendorIndexLookup
    {
        [DataMember(Order = 1)]
        public IList<VendorId> Identities { get; private set; }

        public VendorIndexLookup()
        {
            Identities = new List<VendorId>();
        }
    }
}
