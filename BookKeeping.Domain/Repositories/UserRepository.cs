using BookKeeping.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Domain.Repositories
{
    public class UserRepository : IUserRepository, IRepository<User>
    {
        private User _admin = new User("Адміністратор", "admin");
        private User _seller = new User("Продавець", "seller");

        public UserRepository()
        {
            _admin.Id = 11;
            _admin.Role = RoleType.Admin;
            _admin.SetPassword("qwerty");

            _seller.Id = 12;
            _seller.Role = RoleType.Teacher;
            _seller.SetPassword("qwerty");
        }

        public IEnumerable<User> All()
        {
            yield return _admin;
            yield return _seller;
        }

        public User Get(long id)
        {
            return All().SingleOrDefault(t => t.Id == id);
        }

        public User Get(string login, string password)
        {
            return All().SingleOrDefault(user => user.Login.Equals(login) && user.Password.Check(password));
        }

        public void Save(User user)
        {
            //do nothing
        }
    }
       
}
