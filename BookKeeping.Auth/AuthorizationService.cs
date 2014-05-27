using BookKeeping.Domain.Repositories;
using System.Security.Principal;
using System.Threading;

namespace BookKeeping.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        readonly IUserRepository _repository;

        public AuthenticationService(IUserRepository repository)
        {
            _repository = repository;
        }

        public virtual bool SignIn(string username, string password)
        {
            var user = _repository.Get(username, password);
            if (user == null)
                return false;
            var identity = new UserIdentity(new AccountEntry(user), username);
            Thread.CurrentPrincipal = new GenericPrincipal(identity, identity.GetRoles());
            return true;
        }

        public virtual void SignOut()
        {
            Thread.CurrentPrincipal = null;
        }
    }
}
