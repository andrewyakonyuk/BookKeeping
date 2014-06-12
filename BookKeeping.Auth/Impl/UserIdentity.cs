using System;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Auth
{
    [Serializable]
    public class UserIdentity : MarshalByRefObject, System.Security.Principal.IIdentity, IUserIdentity
    {
        private readonly AccountEntry accountEntry;

        public UserIdentity(AccountEntry accountEntry, string name)
        {
            Name = name;
            this.accountEntry = accountEntry;
        }

        public UserId Id
        {
            get { return accountEntry.Id; }
        }

        public string RoleType
        {
            get { return accountEntry.RoleType; }
        }

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return Id.Id > 0; }
        }

        public string Name { get; private set; }

        public string[] GetRoles()
        {
            return new[] { RoleType };
        }
    }
}