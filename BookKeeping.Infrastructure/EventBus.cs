using BookKeeping.Core.Domain;
using System;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure
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
            foreach (var eventHandler in handlers)
            {
                _queue.Enqueue(() =>
                {
                    eventHandler.When(@event);
                });
            }
            _isCommited = false;
        }

        public void Commit()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()();
            }
            _isCommited = true;
        }

        public void Rollback()
        {
            _queue.Clear();
            _isCommited = false;
        }

        public void Dispose()
        {
            if (!_isCommited)
                Rollback();
            GC.SuppressFinalize(this);
        }
    }
}