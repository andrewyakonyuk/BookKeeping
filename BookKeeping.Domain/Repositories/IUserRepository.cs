using BookKeeping.Domain.Aggregates;

namespace BookKeeping.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User Get(string login, string password);
    }
}
