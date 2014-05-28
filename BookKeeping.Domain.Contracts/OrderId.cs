using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [DataContract(Namespace = "BookKeeping")]
    [Serializable]
    public sealed class OrderId : IdentityBase<long>, IIdentity
    {
        public OrderId()
            : base(0)
        {

        }

        public OrderId(long id)
            : base(id)
        {

        }

        public const string Tag = "order";

        public override string GetTag()
        {
            return Tag;
        }

        public override string ToString()
        {
            return string.Format("order-{0}", Id);
        }

        [DataMember(Order = 1)]
        public override long Id { get; protected set; }
    }
}
