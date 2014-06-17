using BookKeeping.Domain.Contracts;
using System;

namespace BookKeeping.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
        void RegisterForTracking<TAggregate>(TAggregate aggregateRoot, IIdentity id)
            where TAggregate : AggregateBase;
        TAggregate Get<TAggregate>(IIdentity id)
            where TAggregate : AggregateBase;
    }
}
