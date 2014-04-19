using BookKeeping.Core;

namespace BookKeeping.Domain.OrderAggregate
{
    public sealed class OrderId : IdentityBase<long>, IIdentity
    {
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
