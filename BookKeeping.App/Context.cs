using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Infrastructure;
using BookKeeping.Infrastructure.AtomicStorage;
using BookKeeping.Infrastructure.Storage;
using System.IO;
using System;
using BookKeeping.Domain.Contracts;
using System.Linq;

namespace BookKeeping.App
{
    public sealed class Context
    {
        readonly IEventStore _eventStore;
        readonly ICommandBus _commandBus;
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

            var eventBus = new EventBus(new EventHandlerFactoryImpl(_projections));
            _commandBus = new CommandBus(new CommandHandlerFactoryImpl(_projections, _eventStore, eventBus));

            _cacheService = CacheService.Current;
        }

        static Type[] LoadMessageContracts()
        {
            var messages = new[] { typeof(CreateWarehouse) }
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

        public void Send<TCommand>(TCommand command)
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
    }
}
