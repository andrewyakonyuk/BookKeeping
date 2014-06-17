using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Projections.UserList
{
    public class UsersProjection :
        IEventHandler<UserCreated>,
        IEventHandler<UserDeleted>,
        IEventHandler<RoleAssignedToUser>,
        IEventHandler<UserRenamed>
    {
        private readonly IDocumentWriter<unit, UserListView> _store;

        public UsersProjection(IDocumentWriter<unit, UserListView> store)
        {
            _store = store;
        }

        public void When(RoleAssignedToUser e)
        {
            _store.UpdateOrThrow(unit.it, u => u.Users.Single(t => t.Id == e.Id).RoleType = e.Role);
        }

        public void When(UserDeleted e)
        {
            _store.UpdateOrThrow(unit.it, u => u.Users.Remove(u.Users.Single(t => t.Id == e.Id)));
        }

        public void When(UserCreated e)
        {
            _store.UpdateEnforcingNew(unit.it, u => u.Users.Add(new UserView
            {
                Id = e.Id,
                Name = e.Name,
                Login = e.Login,
                RoleType = e.Role
            }));
        }

        public void When(UserRenamed e)
        {
            _store.UpdateOrThrow(unit.it, u => u.Users.Find(t => t.Id == e.Id).Name = e.NewName);
        }
    }
}
