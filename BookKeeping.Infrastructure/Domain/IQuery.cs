using BookKeeping.Domain.Contracts;
namespace BookKeeping.Domain
{
    public interface IQuery<in TCriterion,TResult>
        where TCriterion : ICriterion
    {
        Maybe<TResult> Ask(TCriterion criterion);
    }
}
