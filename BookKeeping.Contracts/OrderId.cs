using BookKeeping.Core;
using BookKeeping.Core.Domain;
using System;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    public sealed class OrderId : IdentityBase<long>, IIdentity
    {
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
    }
}
