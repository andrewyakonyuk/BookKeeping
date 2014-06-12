using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure.Domain.Impl
{
    public class EventBus : IEventBus
    {
        private IEventHandlerFactory _eventHandlerFactory;
        Queue<Action> _queue = new Queue<Action>();
        private bool _isCommited = false;

        public EventBus(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
        }

        public void Publish<T>(T @event)
            where T : IEvent
        {
            var handlers = _eventHandlerFactory.GetHandlers<T>();
            foreach (var handler in handlers)
            {
                handler.When(@event);
            }
        }
    }
}
