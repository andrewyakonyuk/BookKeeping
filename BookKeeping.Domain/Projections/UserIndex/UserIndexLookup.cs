using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Projections.UserIndex
{
    [DataContract]
    public sealed class UserIndexLookup
    {
        [DataMember(Order = 1)]
        public IDictionary<string, UserId> Logins { get; private set; }

        [DataMember(Order = 3)]
        public IList<UserId> Identities { get; private set; }

        public UserIndexLookup()
        {
            Logins = new Dictionary<string, UserId>(StringComparer.InvariantCultureIgnoreCase);
            Identities = new List<UserId>();
        }
    }
}
