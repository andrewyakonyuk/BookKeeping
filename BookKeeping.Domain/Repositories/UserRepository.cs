using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain.Repositories
{
    public class UserRepository : RepositoryBase<User,UserId>, IUserRepository, IRepository<User, UserId>
    {
        readonly IEventStore _eventStore;
        readonly IDocumentReader<unit, UserIndexLookup> _userIndexReader;
        readonly IEventBus _eventBus;

        public UserRepository(IEventStore eventStore, IEventBus eventBus, IDocumentReader<unit, UserIndexLookup> userIndexReader)
            : base(eventStore, eventBus)
        {
            _eventStore = eventStore;
            _userIndexReader = userIndexReader;
            _eventBus = eventBus;
        }

        public override IEnumerable<User> All()
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

        public override User Get(UserId id)
        {
            var stream = _eventStore.LoadEventStream(id);
            return new User(stream.Events);
        }

        public override User Load(UserId id)
        {
            var stream = _eventStore.LoadEventStream(id);
            if (stream.Version > 0)
                return new User(stream.Events);
            return null;
        }

        public virtual User Load(string login, string password)
        {
            var user = _userIndexReader.Get<UserIndexLookup>()
                .Convert(t => t.Logins.ContainsKey(login) ? t.Logins[login] : new UserId(-1))
                .Convert(t => Load(t), default(User));
            if (user == null)
                return null;
            if (user.Password.Check(password))
            {
                return user;
            }
            else return null;
        }
    }
}
