using BookKeeping.Core.Domain;
using System;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure
{
    public class EventBus : IEventBus
    {
        private IEventHandlerFactory _eventHandlerFactory;
        Queue<Action> _queue = new Queue<Action>();

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
                _queue.Enqueue(() =>
                {
                    eventHandler.When(@event);
                });
            }
        }

        public void Commit()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()();
            }
        }

        public void Rollback()
        {
            _queue.Clear();
        }
    }
}