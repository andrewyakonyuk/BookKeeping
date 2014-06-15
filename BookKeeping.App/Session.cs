using BookKeeping.Domain;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Infrastructure.Domain.Impl;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using System;

namespace BookKeeping.App
{
    public class Session : ISession, IUnitOfWork, IDomainIdentityService
    {
        readonly IEventStore _eventStore;
        readonly IEventBus _eventBus;
        readonly ICommandBus _commandBus;
        readonly IDocumentStore _projections;
        readonly IUnitOfWork _innerUnitOfWork;
        readonly IDomainIdentityService _identityGenerator;

        public Session(IEventStore eventStore, IEventBus eventBus, IDocumentStore projections)
        {
            _eventBus = eventBus;
            _eventStore = eventStore;
            _projections = projections;
            _innerUnitOfWork = new UnitOfWork(_eventStore, _eventBus);
            _commandBus = new CommandBus(new CommandHandlerFactoryImpl(_projections, _innerUnitOfWork, _eventStore, _eventBus));
            _identityGenerator = new DomainIdentityGenerator(_projections);
        }

        public void Command<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            _commandBus.Send(command);
        }

        public Maybe<TView> Query<TKey, TView>(TKey id)
        {
            return _projections.GetReader<TKey, TView>().Get(id);
        }

        public Maybe<TView> Query<TView>()
        {
            return Query<unit, TView>(unit.it);
        }

        [Obsolete]
        public IRepository<T, TId> GetRepo<T, TId>()
            where T : AggregateBase
            where TId : IIdentity
        {
            //todo: 
            if (typeof(T) == typeof(User) && typeof(TId) == typeof(UserId))
            {
                return (IRepository<T, TId>)new UserRepository(_eventStore, _innerUnitOfWork, _projections.GetReader<unit, UserIndexLookup>());
            }
            return null;
        }

        public void Commit()
        {
            _innerUnitOfWork.Commit();
        }

        public void Rollback()
        {
            _innerUnitOfWork.Rollback();
        }

        public void RegisterForTracking<TAggregate>(TAggregate aggregateRoot, IIdentity id)
            where TAggregate : AggregateBase
        {
            _innerUnitOfWork.RegisterForTracking(aggregateRoot, id);
        }

        public TAggregate Get<TAggregate>(IIdentity id) where TAggregate : AggregateBase
        {
            return _innerUnitOfWork.Get<TAggregate>(id);
        }

        public long GetId()
        {
            return _identityGenerator.GetId();
        }

        public void Dispose()
        {
            _innerUnitOfWork.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
