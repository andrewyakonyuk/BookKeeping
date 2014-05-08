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
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookKeeping.App
{
    public sealed class Context
    {
        readonly IEventStore _eventStore;
        readonly ICommandBus _commandBus;
        readonly IEventBus _eventBus;
        readonly IDocumentStore _projections;
        readonly ICacheService _cacheService;
        static Context _this;
        static object _lock = new object();
        readonly Stopwatch _stopwatch = new Stopwatch();
        ContextUnitOfWork _unitOfWork;

        public Context()
        {
            //TODO: move to configuration
            var pathToStore = Path.Combine(Directory.GetCurrentDirectory(), "store");
            var appendOnlyStore = new FileAppendOnlyStore(pathToStore);
            appendOnlyStore.Initialize();

            _eventStore = new EventStore(appendOnlyStore, new DefaultMessageStrategy(LoadMessageContracts()));
            _projections = new FileDocumentStore(pathToStore, new DefaultDocumentStrategy());

            _eventBus = new EventBus(new EventHandlerFactoryImpl(_projections));
            _commandBus = new CommandBus(new CommandHandlerFactoryImpl(_projections, _eventStore, _eventBus));

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
            Profile(() => _commandBus.Send(command), command.ToString());
            if (_unitOfWork != null)
                _unitOfWork.Reset();
        }

        public Maybe<TView> Query<TKey, TView>(TKey id)
        {
            return Profile(() => _projections.GetReader<TKey, TView>().Get(id), typeof(TView).Name);
        }

        public Maybe<TView> Query<TView>()
        {
            return Query<unit, TView>(unit.it);
        }

        private TResult Profile<TResult>(Func<TResult> func, string name)
        {
            Trace.TraceInformation("Begin query {0}: ", name);
            _stopwatch.Restart();
            var result = func();
            _stopwatch.Stop();
            Trace.TraceInformation("End. Ellapsed: {0}", _stopwatch.Elapsed);
            return result;
        }

        private void Profile(Action action, string name)
        {
            Trace.TraceInformation("Start command {0}: ", name);
            _stopwatch.Restart();
            action();
            _stopwatch.Stop();
            Trace.TraceInformation("End. Ellapsed: {0}", _stopwatch.Elapsed);
        }

        public IUnitOfWork Capture()
        {
            if (_unitOfWork != null)
            {
                _unitOfWork.Dispose();
            }
            _unitOfWork = new ContextUnitOfWork(_commandBus);
            return _unitOfWork;
        }

        private class ContextUnitOfWork : IUnitOfWork, IDisposable
        {
            bool _isCommited = false;
            ICommandBus _commandBus;

            public ContextUnitOfWork(ICommandBus commandBus)
            {
                _commandBus = commandBus;
            }

            public void Commit()
            {
                try
                {
                    _commandBus.Commit();
                    _isCommited = true;
                }
                catch (Exception)
                {
                    Rollback();
                    throw;
                }
            }

            public void Reset()
            {
                _isCommited = false;
            }

            public void Rollback()
            {
                _commandBus.Rollback();
            }

            public void Dispose()
            {
                if (!_isCommited)
                    Rollback();
            }
        }
    }
}
