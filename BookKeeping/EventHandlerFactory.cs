using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping
{
    internal sealed class EventHandlerFactoryImpl : IEventHandlerFactory
    {
        private readonly IDocumentStore _documentStore;

        public EventHandlerFactoryImpl(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent
        {
            return DomainBoundedContext.Projections(_documentStore).OfType<IEventHandler<T>>();
        }
    }
}