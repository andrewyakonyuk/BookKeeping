using BookKeeping.Core;
using BookKeeping.Core.Domain;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent;
    }
}