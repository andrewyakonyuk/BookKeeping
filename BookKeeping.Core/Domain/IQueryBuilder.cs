namespace BookKeeping.Core.Domain
{
    public interface IQueryBuilder
    {
        IQueryFor<TResult> For<TResult>();
    }
}
