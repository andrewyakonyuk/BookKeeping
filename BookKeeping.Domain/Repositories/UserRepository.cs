using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public class UserRepository : IUserRepository, IRepository<User, UserId>
    {
        readonly IEventStore _eventStore;
        readonly IDocumentReader<unit, UserIndexLookup> _userIndexReader;

        public UserRepository(IEventStore eventStore, IDocumentReader<unit, UserIndexLookup> userIndexReader)
        {
            _eventStore = eventStore;
            _userIndexReader = userIndexReader;
        }

        public IEnumerable<User> All()
        {
            var index = _userIndexReader.Get<UserIndexLookup>();
            if (index.HasValue)
            {
                foreach (var item in index.Value.Identities)
                {
                    yield return Get(item);
                }
            }
            yield break;
        }

        public User Get(UserId id)
        {
            var stream = _eventStore.LoadEventStream(id);
            if (stream.Version > 0)
                return new User(stream.Events);
            return null;
        }

        public User Get(string login, string password)
        {
            var user = _userIndexReader.Get<UserIndexLookup>()
                .Convert(t => t.Logins[login])
                .Convert(t => Get(t), default(User));
            if (user != null && user.Password.Check(password))
            {
                return user;
            }
            return null;
        }

        public void Save(User user)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(user.Id);
                try
                {
                    _eventStore.AppendToStream(user.Id, eventStream.Version, user.Changes);
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var clientEvent in user.Changes)
                    {
                        foreach (var actualEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(clientEvent, actualEvent))
                            {
                                throw new RealConcurrencyException(string.Format("Conflict between {0} and {1}", clientEvent, actualEvent), ex);
                            }
                        }
                    }
                    // there are no conflicts and we can append
                    _eventStore.AppendToStream(user.Id, ex.ActualVersion, user.Changes);
                }
            }
        }

        static bool ConflictsWith(IEvent x, IEvent y)
        {
            return x.GetType() == y.GetType();
        }
    }
}
