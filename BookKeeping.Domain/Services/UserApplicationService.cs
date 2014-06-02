using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Domain.Repositories;
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
        readonly IRepository<User, UserId> _repository;
        private readonly IDocumentReader<unit, UserIndexLookup> _userIndex;

        public UserApplicationService(IRepository<User,UserId> repository, IDocumentReader<unit, UserIndexLookup> userIndex)
        {
            _repository = repository;
            _userIndex = userIndex;
        }

        private void Update(UserId id, Action<User> execute)
        {
            var user = _repository.Get(id);
            execute(user);
            _repository.Save(user);
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
