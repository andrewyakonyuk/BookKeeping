using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Infrastructure;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using System;

namespace BookKeeping.Domain.Services
{
    public class UserApplicationService : IUserApplicationService,
        ICommandHandler<CreateUser>,
        ICommandHandler<AssignRoleToUser>,
        ICommandHandler<ChangeUserPassword>,
        ICommandHandler<DeleteUser>
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IDocumentReader<unit, UserIndexLookup> _userIndex;

        public UserApplicationService(IEventStore eventStore, IEventBus eventBus, IDocumentReader<unit, UserIndexLookup> userIndex)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _userIndex = userIndex;
        }

        private void Update(UserId id, Action<User> execute)
        {
            var stream = _eventStore.LoadEventStream(id);
            var user = new User(stream.Events);
            execute(user);
            _eventStore.AppendToStream(id, stream.Version, user.Changes);

            foreach (var @event in user.Changes)
            {
                var realEvent = (dynamic)System.Convert.ChangeType(@event, @event.GetType());
                _eventBus.Publish(realEvent);
            }
        }

        public void When(CreateUser c)
        {
            if (_userIndex.Get<UserIndexLookup>().Convert(t => t.Logins.ContainsKey(c.Login), false))
            {
                throw new InvalidOperationException(string.Format("User with same login '{0}' already exists", c.Login));
            }
            Update(c.Id, u => u.Create(c.Id, c.Name, c.Login, c.Password, c.Role, Current.UtcNow));
        }

        public void When(AssignRoleToUser c)
        {
            Update(c.Id, u => u.AssignToRole(c.Role, Current.UtcNow));
        }

        public void When(ChangeUserPassword c)
        {
            Update(c.Id, u => u.ChangePassword(c.OldPassword, c.NewPassword, Current.UtcNow));
        }

        public void When(DeleteUser c)
        {
            Update(c.Id, u => u.Delete(Current.UtcNow));
        }
    }
}
