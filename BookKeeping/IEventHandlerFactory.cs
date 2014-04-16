using BookKeeping.Core;
using System.Collections.Generic;

namespace BookKeeping
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent;
    }
}