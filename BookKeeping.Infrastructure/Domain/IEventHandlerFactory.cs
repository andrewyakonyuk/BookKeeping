using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Domain
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent;
    }
}
