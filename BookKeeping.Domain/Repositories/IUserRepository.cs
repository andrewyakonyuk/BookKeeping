using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, UserId>
    {
        User Load(string login, string password);
    }
}
