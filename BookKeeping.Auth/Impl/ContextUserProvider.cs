using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Repositories;
using System.Threading;

namespace BookKeeping.Auth
{
    public class ContextUserProvider : IContextUserProvider
    {
        private readonly IRepository<User,UserId> userRepository;

        public ContextUserProvider(IRepository<User,UserId> userRepository)
        {
            this.userRepository = userRepository;
        }

        public User ContextUser()
        {
            if (Thread.CurrentPrincipal == null)
            {
                return null;
            }
            var identity = Thread.CurrentPrincipal.Identity as UserIdentity;
            if (identity == null)
            {
                return null;
            }
            return userRepository.Get(identity.Id);
        }
    }
}
