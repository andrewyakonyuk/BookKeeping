﻿using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Contracts.Store.Commands;
using BookKeeping.Infrastructure;
using BookKeeping.Infrastructure.AtomicStorage;
using BookKeeping.Infrastructure.Storage;
using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            var messages = new[] { typeof(CreateStore) }
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

        public void Command<TCommand>(TCommand command)
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
            _unitOfWork = new ContextUnitOfWork(_eventBus);
            return _unitOfWork;
        }

        private sealed class CommandHandlerFactoryImpl : ICommandHandlerFactory
        {
            private readonly IDocumentStore _documentStore;
            private readonly IEventBus _eventBus;
            private readonly IEventStore _eventStore;

            public CommandHandlerFactoryImpl(IDocumentStore documentStore, IEventStore eventStore, IEventBus eventBus)
            {
                _documentStore = documentStore;
                _eventBus = eventBus;
                _eventStore = eventStore;
            }

            public ICommandHandler<T> GetHandler<T>() where T : ICommand
            {
                return (ICommandHandler<T>)DomainBoundedContext.EntityApplicationServices(_documentStore, _eventStore, _eventBus)
                    .SingleOrDefault(service => service is ICommandHandler<T>);
            }
        }

        private sealed class EventHandlerFactoryImpl : IEventHandlerFactory
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

        private class ContextUnitOfWork : IUnitOfWork, IDisposable
        {
            bool _isCommited = false;
            IEventBus _eventBus;

            public ContextUnitOfWork(IEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public void Commit()
            {
                try
                {
                    _eventBus.Commit();
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
                _eventBus.Rollback();
            }

            public void Dispose()
            {
                if (!_isCommited)
                    Rollback();
            }
        }
    }
}
