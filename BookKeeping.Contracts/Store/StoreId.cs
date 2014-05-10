using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts.Store
{
    [DataContract(Namespace = "BookKeeping")]
    [Serializable]
    public sealed class StoreId : IdentityBase<Guid>, IIdentity
    {
        [Obsolete("Only for serializer")]
        protected StoreId()
            : base(Guid.Empty)
        {

        }

        public StoreId(Guid id)
            : base(id)
        {

        }

        public override string GetTag()
        {
            return "store";
        }

        public override string ToString()
        {
            return string.Concat(GetTag(), "-", Id);
        }

        [DataMember(Order = 1)]
        public override Guid Id { get; protected set; }
    }
}
