namespace BookKeeping.Infrastructure.NHibernate
{
    using System.Linq;
    using Domain;
    

    /// <summary>
    /// </summary>
    /// <typeparam name="TCriterion"> </typeparam>
    /// <typeparam name="TResult"> </typeparam>
   
    public abstract class LinqQueryBase<TCriterion, TResult> : IQuery<TCriterion, TResult>
        where TCriterion : ICriterion
    {
        private readonly ILinqProvider _linq;

        protected LinqQueryBase(ILinqProvider linq)
        {
            _linq = linq;
        }

        #region IQuery<TCriterion,TResult> Members

        public abstract TResult Ask(TCriterion criterion);

        #endregion

        /// <summary>
        /// </summary>
       
        public virtual IQueryable<TEntity> Query<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return _linq.Query<TEntity>();
        }
    }
}