using BookKeeping.Core;
using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
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

        [DataMember(Order = 1)]
        public override Guid Id { get; protected set; }
    }
}
