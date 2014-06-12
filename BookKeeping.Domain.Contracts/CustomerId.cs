using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    /// <summary>
    /// This is a customer identity. It is just a class that makes it explicit,
    /// that this specific <em>long</em> is not just any number, but an identifier
    /// of a customer aggregate. This has a lot of benefits in further development.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class CustomerId : IdentityBase<long>, IIdentity
    {
        public const string Tag = "customer";

        public CustomerId()
            : base(0)
        {

        }

        public CustomerId(long id) : base(id) { }

        public override string ToString()
        {
            return string.Format("customer-{0}", Id);
        }

        public override string GetTag()
        {
            return Tag;
        }

        [DataMember(Order = 1)]
        public override long Id { get; protected set; }
    }
}
