namespace BookKeeping.Infrastructure.NHibernate
{
    using System.Linq;
    using Domain;
    

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    /// <typeparam name="TCriterion"> </typeparam>
    /// <typeparam name="TResult"> </typeparam>
   
    public abstract class LinqQueryBase<TEntity, TCriterion, TResult> : IQuery<TCriterion, TResult>
        where TCriterion : ICriterion
        where TEntity : class, IEntity, new()
    {
        private readonly ILinqProvider _linq;

        protected LinqQueryBase(ILinqProvider linq)
        {
            _linq = linq;
        }

        /// <summary>
        /// </summary>
       
        public virtual IQueryable<TEntity> Query
        {
            get { return _linq.Query<TEntity>(); }
        }

        #region IQuery<TCriterion,TResult> Members

        public abstract TResult Ask(TCriterion criterion);

        #endregion
    }
}