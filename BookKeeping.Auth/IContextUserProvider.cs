using BookKeeping.Domain.Aggregates;

namespace BookKeeping.Auth
{
    public interface IContextUserProvider
    {
        User ContextUser();
    }
}
