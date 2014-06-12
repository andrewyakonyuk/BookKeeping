namespace BookKeeping.Infrastructure.Domain
{
    public interface IQueryBuilder
    {
        IQueryFor<TResult> For<TResult>();
    }
}
