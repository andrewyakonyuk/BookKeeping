namespace BookKeeping.Domain
{
    public interface IQueryBuilder
    {
        IQueryFor<TResult> For<TResult>();
    }
}
