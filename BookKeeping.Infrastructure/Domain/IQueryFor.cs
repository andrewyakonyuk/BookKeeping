namespace BookKeeping.Infrastructure.Domain
{
    public interface IQueryFor<out T>
    {
        T With<TCriterion>(TCriterion criterion)
            where TCriterion : ICriterion;
    }
}
