using System;

namespace BookKeeping.Infrastructure.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
