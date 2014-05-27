using BookKeeping.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace BookKeeping.Auth
{
    [Serializable]
    public class UserIdentity : MarshalByRefObject, IIdentity
    {
        private readonly AccountEntry accountEntry;

        public UserIdentity(AccountEntry accountEntry, string name)
        {
            Name = name;
            this.accountEntry = accountEntry;
        }

        public long Id
        {
            get { return accountEntry.Id; }
        }

        public RoleType RoleType
        {
            get { return accountEntry.RoleType; }
        }

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return Id > 0; }
        }

        public string Name { get; private set; }

        public string[] GetRoles()
        {
            return new[] { RoleType.ToString() };
        }
    }
}
