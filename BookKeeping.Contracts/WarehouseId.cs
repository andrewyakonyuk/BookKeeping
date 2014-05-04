using BookKeeping.Core.Domain;
using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [DataContract(Namespace = "BookKeeping")]
    [Serializable]
    public sealed class WarehouseId : IdentityBase<Guid>, IIdentity
    {
        [Obsolete("Only for serializer")]
        protected WarehouseId()
            : base(Guid.Empty)
        {

        }

        public WarehouseId(Guid id)
            : base(id)
        {

        }

        public override string GetTag()
        {
            return "warehouse";
        }

        public override string ToString()
        {
            return string.Concat(GetTag(), "-", Id);
        }

        [DataMember(Order = 1)]
        public override Guid Id { get; protected set; }
    }
}
