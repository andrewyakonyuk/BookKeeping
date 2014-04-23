namespace BookKeeping.Infrastructure.NHibernate
{
    using System;
    
    using global::NHibernate;

    /// <summary>
    /// 
    /// </summary>
   
    public class StaticSessionProvider : ISessionProvider
    {
        private readonly object _lockObject = new object();
        private readonly ISession _session;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StaticSessionProvider( ISession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            _session = session;
        }

        #region ISessionProvider Members

        public ISession CurrentSession
        {
            get
            {
                lock (_lockObject)
                {
                    return _session;
                }
            }
        }

        #endregion
    }
}