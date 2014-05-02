namespace BookKeeping.Core.Domain
{
    public interface IQuery<in TCriterion, out TResult>
        where TCriterion : ICriterion
    {
        TResult Ask(TCriterion criterion);
    }
}
