namespace BookKeeping.Infrastructure.NHibernate
{
    using Domain;
    

    /// <summary>
    ///   ����������� ����������� ��� NHibernate
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    
    public sealed class NHibernateRepository<TEntity> : NHibernateRepositoryBase<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        ///   �����������
        /// </summary>
        /// <param name="sessionProvider"> </param>
        public NHibernateRepository(ISessionProvider sessionProvider)
            : base(sessionProvider)
        {
        }
    }
}