using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;

namespace BookKeeping.Domain
{
    public interface ICommandHandlerProvider
    {
        IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus, IUnitOfWork unitOfWork);
    }
}
