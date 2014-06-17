using BookKeeping.Domain.Contracts;
using BookKeeping.Persistance.AtomicStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain
{
    public sealed class EventHandlerFactoryEvil : IEventHandlerFactory
    {
        private readonly IDocumentStore _documentStore;
        private static IEventHandlerProvider[] _handlerProviders;

        static EventHandlerFactoryEvil()
        {
            _handlerProviders = AppDomain.CurrentDomain.GetAssemblies().Where(t => t.FullName.StartsWith("BookKeeping"))
                 .SelectMany(t => t.GetExportedTypes())
                 .Where(t => typeof(IEventHandlerProvider).IsAssignableFrom(t) && !t.IsAbstract)
                 .Select(t => (IEventHandlerProvider)Activator.CreateInstance(t))
                 .ToArray();
        }

        public EventHandlerFactoryEvil(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent
        {
            return _handlerProviders.SelectMany(t => t.Projections(_documentStore))
                .OfType<IEventHandler<T>>();
        }
    }
}
