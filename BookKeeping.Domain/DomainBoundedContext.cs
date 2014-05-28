using BookKeeping.Domain.Aggregates.Customer;
using BookKeeping.Domain.Aggregates.Product;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using System.Collections.Generic;

namespace BookKeeping
{
    public class DomainBoundedContext
    {
        public static IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield break;
        }

        public static IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus)
        {
            yield return new CustomerApplicationService(store, eventBus, new PricingService());
            yield return new ProductApplicationService(store, eventBus);
        }
    }
}
