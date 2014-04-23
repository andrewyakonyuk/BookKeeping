using BookKeeping.Core;
using BookKeeping.Infrastructure.Domain;
using System;

namespace BookKeeping.Domain.ProductAggregate
{
    [Serializable]
    public sealed class ProductId : IdentityBase<Guid>, IIdentity
    {
        public ProductId(Guid id)
            : base(id)
        {

        }

        public const string Tag = "product";

        public override string GetTag()
        {
            return Tag;
        }

        public override string ToString()
        {
            return string.Format("product-{0}", Id);
        }
    }
}
