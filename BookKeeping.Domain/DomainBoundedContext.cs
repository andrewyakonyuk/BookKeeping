using BookKeeping.Domain.Projections.CustomerIndex;
using BookKeeping.Domain.Projections.OrderIndex;
using BookKeeping.Domain.Projections.ProductIndex;
using BookKeeping.Domain.Projections.UserIndex;
using BookKeeping.Domain.Projections.VendorIndex;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Persistance.Storage;
using System.Collections.Generic;

namespace BookKeeping
{
    public class DomainBoundedContext : ICommandHandlerProvider, IEventHandlerProvider
    {
        public IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new UserIndexProjection(docs.GetWriter<unit, UserIndexLookup>());
            yield return new ProductIndexProjection(docs.GetWriter<unit, ProductIndexLookup>());
            yield return new CustomerIndexProjection(docs.GetWriter<unit, CustomerIndexLookup>());
            yield return new VendorIndexProjection(docs.GetWriter<unit, VendorIndexLookup>());
        }

        public IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            yield return new CustomerApplicationService(new CustomerRepository(store, unitOfWork, docs.GetReader<unit, CustomerIndexLookup>()), new PricingService());
            yield return new ProductApplicationService(new ProductRepository(store, unitOfWork, docs.GetReader<unit, ProductIndexLookup>()));
            yield return new UserApplicationService(new UserRepository(store, unitOfWork, docs.GetReader<unit, UserIndexLookup>()), docs.GetReader<unit, UserIndexLookup>());
            yield return new VendorApplicationService(new VendorRepository(store, unitOfWork, docs.GetReader<unit, VendorIndexLookup>()), new PricingService());
            yield return new OrderApplicationService(new OrderRepository(store, unitOfWork, docs.GetReader<unit, OrderIndexLookup>()));
        }
    }
}
