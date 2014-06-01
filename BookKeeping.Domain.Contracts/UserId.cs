using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    [DataContract(Namespace = "BookKeeping")]
    public partial class UserId : IdentityBase<long>, IIdentity
    {
        [DataMember(Order = 1)]
        public override long Id { get; protected set; }

        public UserId()
            : base(0)
        {

        }

        public UserId(long id)
            : base(id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return string.Format(@"User {0}", Id);
        }

        public override string GetTag()
        {
            return "user";
        }
    }
}
