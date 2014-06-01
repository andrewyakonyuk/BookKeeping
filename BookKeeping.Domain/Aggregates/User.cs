using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace BookKeeping.Domain.Aggregates
{
    public class User : AggregateBase, IUserState
    {
        public UserId Id { get; private set; }

        public Password Password { get; private set; }

        public string Name { get; private set; }

        public string Login { get; private set; }

        public string Role { get; private set; }

        public User(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public void Create(UserId id, string name, string login, string password, string role, DateTime utc)
        {
            Apply(new UserCreated(id, name, login, new Password(password), role, utc));
        }

        public void AssignToRole(string role, DateTime utc)
        {
            Apply(new RoleAssignedToUser(this.Id, role, utc));
        }

        public void Delete(DateTime utc)
        {
            Apply(new UserDeleted(this.Id, utc));
        }

        public void ChangePassword(string oldPassword, string newPassword, DateTime utc)
        {
            if (this.Password.Check(oldPassword))
            {
                Apply(new UserPasswordChanged(this.Id, new Password(newPassword), utc));
            }
            else throw new InvalidOperationException();
        }



        public void When(UserCreated e)
        {
            Id = e.Id;
            Password = e.Password;
            Name = e.Name;
            Login = e.Login;
            Role = e.Role;
        }

        public void When(RoleAssignedToUser e)
        {
            Role = e.Role;
        }

        public void When(UserDeleted e)
        {
            Version = -1;
        }

        public void When(UserPasswordChanged e)
        {
            Password = e.Password;
        }

        
    }
}
