using BookKeeping.Domain;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Infrastructure.Domain.Impl;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookKeeping.App
{
    public sealed class Context
    {
        readonly IEventStore _eventStore;
        readonly IEventBus _eventBus;
        readonly IDocumentStore _projections;
        readonly ICacheService _cacheService;
        static Context _this;
        static object _lock = new object();

        public Context()
        {
            //TODO: move to configuration
            var pathToStore = Path.Combine(Directory.GetCurrentDirectory(), "store");
            var appendOnlyStore = new FileAppendOnlyStore(pathToStore);
            appendOnlyStore.Initialize();

            _eventStore = new EventStore(appendOnlyStore, new DefaultMessageStrategy(LoadMessageContracts()));
            _projections = new FileDocumentStore(pathToStore, new DefaultDocumentStrategy());

            _eventBus = new EventBus(new EventHandlerFactoryImpl(_projections));

            _cacheService = CacheService.Current;
        }

        static Type[] LoadMessageContracts()
        {
            var messages = new[] { typeof(CreateUser) }
                .SelectMany(t => t.Assembly.GetExportedTypes())
                .Where(t => typeof(IEvent).IsAssignableFrom(t) || typeof(ICommand).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToArray();
            return messages;
        }

        public static Context Current
        {
            get
            {
                lock (_lock)
                {
                    if (_this == null)
                    {
                        _this = new Context();
                    }
                    return _this;
                }
            }
        }

        public ICacheService Cache { get { return _cacheService; } }

        public Session GetSession()
        {
            return new Session(_eventStore, _eventBus, _projections);
        }
    }

    public class Session : IUnitOfWork, IDomainIdentityService
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

    internal sealed class CommandHandlerFactoryImpl : ICommandHandlerFactory
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandlerFactoryImpl(IDocumentStore documentStore, IUnitOfWork unitOfWork, IEventStore eventStore, IEventBus eventBus)
        {
            _documentStore = documentStore;
            _eventBus = eventBus;
            _eventStore = eventStore;
            _unitOfWork = unitOfWork;
        }

        public ICommandHandler<T> GetHandler<T>() where T : ICommand
        {
            return (ICommandHandler<T>)DomainBoundedContext.EntityApplicationServices(_documentStore, _eventStore, _eventBus, _unitOfWork)
                .SingleOrDefault(service => service is ICommandHandler<T>);
        }
    }

    internal sealed class EventHandlerFactoryImpl : IEventHandlerFactory
    {
        private readonly IDocumentStore _documentStore;

        public EventHandlerFactoryImpl(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent
        {
            return DomainBoundedContext.Projections(_documentStore)
                .Concat(ClientBoundedContext.Projections(_documentStore))
                .OfType<IEventHandler<T>>();
        }
    }
}