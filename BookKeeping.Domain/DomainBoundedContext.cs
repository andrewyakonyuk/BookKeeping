using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Aggregates.Customer;
using BookKeeping.Domain.Aggregates.Product;
using BookKeeping.Domain.Aggregates.Store;
using BookKeeping.Domain.Services;
using BookKeeping.Domain.Services.StoreIndex;
using System.Collections.Generic;

namespace BookKeeping
{
    public class DomainBoundedContext
    {
        public static IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new StoreIndexProjection(docs.GetWriter<string, StoreIndexView>());
            yield break;
        }

        public static IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus)
        {
            var warehouseService = new StoreIndexService(docs.GetReader<string, StoreIndexView>());

            yield return new CustomerApplicationService(store, eventBus, new PricingService());
            yield return new ProductApplicationService(store, eventBus, warehouseService);
            yield return new StoreApplicationService(store, eventBus, warehouseService);
        }
    }
}
