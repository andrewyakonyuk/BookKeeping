using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class VendorId : IdentityBase<long>, IIdentity
    {
        public const string Tag = "vendor";

        public VendorId()
            : base(0)
        {

        }

        public VendorId(long id) : base(id) { }

        public override string ToString()
        {
            return string.Format("vendor-{0}", Id);
        }

        public override string GetTag()
        {
            return Tag;
        }

        [DataMember(Order = 1)]
        public override long Id { get; protected set; }
    }
}
