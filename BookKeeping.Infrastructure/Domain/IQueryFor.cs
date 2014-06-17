using BookKeeping.Domain.Contracts;
namespace BookKeeping.Domain
{
    public interface IQueryFor<T>
    {
        Maybe<T> Ask<TCriterion>(TCriterion criterion)
            where TCriterion : ICriterion;
    }
}
