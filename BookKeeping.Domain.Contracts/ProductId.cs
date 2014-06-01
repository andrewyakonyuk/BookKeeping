using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [DataContract(Namespace = "BookKeeping")]
    public partial class ProductId : IdentityBase<long>, IIdentity
    {
        [DataMember(Order = 1)]
        public override long Id { get; protected set; }

        public ProductId(long id)
            : base(id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return string.Format(@"Product {0}", Id);
        }

        public override string GetTag()
        {
            return "product";
        }
    }
}
