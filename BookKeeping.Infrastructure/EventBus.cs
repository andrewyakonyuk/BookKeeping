using BookKeeping.Core;
using BookKeeping.Core.Domain;
using BookKeeping.Infrastructure;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure
{
    public class EventBus : IEventBus
    {
        private IEventHandlerFactory _eventHandlerFactory;

        public EventBus(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
        }

        public void Publish<T>(T @event) 
            where T : IEvent
        {
            var handlers = _eventHandlerFactory.GetHandlers<T>();
            foreach (var eventHandler in handlers)
            {
                eventHandler.When(@event);
            }
        }
    }
}