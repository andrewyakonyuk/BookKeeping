using BookKeeping.Domain.Contracts;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using BookKeeping.Projections.CustomerList;
using BookKeeping.Projections.CustomerTransactions;
using BookKeeping.Projections.ProductsList;
using BookKeeping.Projections.UserList;
using BookKeeping.Projections.VendorList;
using System.Collections.Generic;
using BookKeeping.Domain;

namespace BookKeeping.Projections
{
    public class ClientBoundedContext : IEventHandlerProvider
    {
        public IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new CustomerTransactionsProjection(docs.GetWriter<CustomerId, CustomerTransactionsListView>());
            yield return new ProductsProjection(docs.GetWriter<unit, ProductListView>());
            yield return new UsersProjection(docs.GetWriter<unit, UserListView>());
            yield return new CustomersProjection(docs.GetWriter<unit, CustomerListView>());
            yield return new VendorsProjection(docs.GetWriter<unit, VendorListView>());
        }
    }
}
