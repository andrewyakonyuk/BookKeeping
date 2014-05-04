using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Core.Storage;
using BookKeeping.Domain.Aggregates.Customer;
using BookKeeping.Domain.Aggregates.Sku;
using BookKeeping.Domain.Services;
using BookKeeping.Domain.Services.WarehouseIndex;
using System.Collections.Generic;

namespace BookKeeping
{
    public class DomainBoundedContext
    {
        public static string EsContainer = "hub-domain-tape";

        public static IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new WarehouseIndexProjection(docs.GetWriter<string, WarehouseIndexView>());
            yield break;
        }

        public static IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus)
        {
            yield return new CustomerApplicationService(store, eventBus, new PricingService());
            yield return new SkuApplicationService(store, eventBus, new WarehouseIndexService(docs.GetReader<string, WarehouseIndexView>()));
        }
    }
}
