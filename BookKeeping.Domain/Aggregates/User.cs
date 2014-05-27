using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Aggregates
{
    public class User : IEntity
    {
        protected User()
        {
        }

        public User(string name, string login)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");

            Name = name;
            Login = login;
        }

        public virtual string Name { get; protected set; }

        public virtual string Login { get; protected set; }

        public virtual RoleType Role { get; set; }

        public virtual Password Password { get; protected set; }

        public virtual long Id { get; set; }

        public virtual void SetPassword(string password)
        {
            if (password == null) throw new ArgumentNullException("password");

            Password = new Password(password);
        }

        public virtual void SetPassword(Password password)
        {
            if (password == null) throw new ArgumentNullException("password");

            Password = password;
        }
    }

    public enum RoleType
    {
        Teacher,
        Student,
        Admin,
        Anonimus
    }
}
