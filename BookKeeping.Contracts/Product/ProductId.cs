using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Product
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class ProductId : IdentityBase<string>, IIdentity
    {
        [Obsolete("Only for serializer")]
        protected ProductId()
            : base(string.Empty)
        {

        }

        public ProductId(string sku)
            : base(sku)
        {

        }

        public override string GetTag()
        {
            return "product";
        }

        [DataMember(Order = 1)]
        public override string Id { get; protected set; }
    }
}
