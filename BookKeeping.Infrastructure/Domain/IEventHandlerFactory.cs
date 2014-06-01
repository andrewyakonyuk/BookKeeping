using BookKeeping.Domain.Contracts;
using System.Collections.Generic;

namespace BookKeeping.Infrastructure.Domain
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : IEvent;
    }
}
