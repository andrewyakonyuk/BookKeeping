using BookKeeping.Domain.Projections.ProductIndex;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using System.Collections.Generic;

namespace BookKeeping
{
    public class DomainBoundedContext
    {
        public static IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new UserIndexProjection(docs.GetWriter<unit, UserIndexLookup>());
            yield return new ProductIndexProjection(docs.GetWriter<unit, ProductIndexLookup>());
        }

        public static IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            yield return new CustomerApplicationService(store, eventBus, new PricingService());
            yield return new ProductApplicationService(new ProductRepository(store, unitOfWork, docs.GetReader<unit, ProductIndexLookup>()));
            yield return new UserApplicationService(new UserRepository(store, unitOfWork, docs.GetReader<unit, UserIndexLookup>()), docs.GetReader<unit, UserIndexLookup>());
        }
    }
}
