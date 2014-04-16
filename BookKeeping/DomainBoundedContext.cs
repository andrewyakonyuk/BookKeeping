using BookKeeping.Core;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Storage;
using BookKeeping.Domain;
using BookKeeping.Domain.CustomerAggregate;
using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping
{
    public class DomainBoundedContext
    {
        public static string EsContainer = "hub-domain-tape";

        public static IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new CustomerTransactionsProjection(docs.GetWriter<CustomerId, CustomerTransactionsDto>());
        }

        public static IEnumerable<object> EntityApplicationServices(IDocumentStore docs, IEventStore store, IEventBus eventBus)
        {
            yield return new CustomerApplicationService(store, eventBus, new PricingService());
            //var unique = new UserIndexService(docs.GetReader<byte, UserIndexLookup>());
            //var passwords = new PasswordGenerator();


            //yield return new UserApplicationService(store);
            //yield return new SecurityApplicationService(store, id, passwords, unique);
            //yield return new RegistrationApplicationService(store, id, unique, passwords);
            //yield return id;
        }
    }
}
