using System;
using System.Collections.Generic;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;

namespace BookKeeping.Domain.Aggregates
{
    public class User : AggregateBase, IUserState
    {
        public User(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public UserId Id { get; private set; }

        public string Login { get; private set; }

        public string Name { get; private set; }

        public Password Password { get; private set; }

        public string Role { get; private set; }

        public void AssignToRole(string role, DateTime utc)
        {
            Apply(new RoleAssignedToUser(this.Id, role, utc));
        }

        public void ChangePassword(string oldPassword, string newPassword, DateTime utc)
        {
            if (this.Password.Check(oldPassword))
            {
                Apply(new UserPasswordChanged(this.Id, new Password(newPassword), utc));
            }
            else throw new InvalidDomainOperationException();
        }

        public void Create(UserId id, string name, string login, string password, string role, DateTime utc)
        {
            Apply(new UserCreated(id, name, login, new Password(password), role, utc));
        }

        public void Delete(DateTime utc)
        {
            Apply(new UserDeleted(this.Id, utc));
        }

        public void Rename(string name, DateTime utc)
        {
            Apply(new UserRenamed(this.Id, name, utc));
        }

        void IUserState.When(UserCreated e)
        {
            Id = e.Id;
            Password = e.Password;
            Name = e.Name;
            Login = e.Login;
            Role = e.Role;
        }

        void IUserState.When(RoleAssignedToUser e)
        {
            Role = e.Role;
        }

        void IUserState.When(UserDeleted e)
        {
            Version = -1;
        }

        void IUserState.When(UserPasswordChanged e)
        {
            Password = e.Password;
        }

        void IUserState.When(UserRenamed e)
        {
            Name = e.NewName;
        }

        protected override void Mutate(IEvent e)
        {
            Version += 1;
            ((IUserState)this).When((dynamic)e);
        }
    }
}