namespace BookKeeping.Infrastructure.NHibernate
{
    using System.Linq;
    using Domain;
    
    using global::NHibernate.Linq;

    /// <summary>
    /// 
    /// </summary>
   
    public class NHibernateLinqProvider : ILinqProvider
    {
        private readonly ISessionProvider _sessionProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionProvider"></param>
        public NHibernateLinqProvider(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        #region ILinqProvider Members

        public IQueryable<TEntity> Query<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return _sessionProvider.CurrentSession.Query<TEntity>();
        }

        #endregion
    }
}