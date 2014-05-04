using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public sealed class SkuId : IdentityBase<string>, IIdentity
    {
        [Obsolete("Only for serializer")]
        protected SkuId()
            : base(string.Empty)
        {

        }

        public SkuId(string sku)
            : base(sku)
        {

        }

        public override string GetTag()
        {
            return "sku";
        }

        [DataMember(Order = 1)]
        public override string Id { get; protected set; }
    }
}
