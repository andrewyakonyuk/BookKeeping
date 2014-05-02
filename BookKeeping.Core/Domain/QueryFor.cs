namespace BookKeeping.Core.Domain
{
    public class QueryFor<TResult> : IQueryFor<TResult>
    {
        private readonly IQueryFactory _factory;

        public QueryFor(IQueryFactory factory)
        {
            _factory = factory;
        }

        public TResult With<TCriterion>(TCriterion criterion)
            where TCriterion : ICriterion
        {
            return _factory.Create<TCriterion, TResult>().Ask(criterion);
        }
    }
}
