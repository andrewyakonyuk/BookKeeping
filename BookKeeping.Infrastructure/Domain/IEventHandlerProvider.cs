using BookKeeping.Persistance.AtomicStorage;
using System.Collections.Generic;

namespace BookKeeping.Domain
{
    public interface IEventHandlerProvider
    {
        IEnumerable<object> Projections(IDocumentStore docs);
    }
}
