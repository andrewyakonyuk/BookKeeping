using BookKeeping.Domain.Contracts;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Projections.CustomerTransactions;
using BookKeeping.Projections.ProductsList;
using BookKeeping.Projections.UserList;
using System.Collections.Generic;

namespace BookKeeping.Projections
{
    public static class ClientBoundedContext
    {
        public static IEnumerable<object> Projections(IDocumentStore docs)
        {
            yield return new CustomerTransactionsProjection(docs.GetWriter<CustomerId, CustomerTransactionsListView>());
            yield return new ProductsProjection(docs.GetWriter<unit, ProductListView>());
            yield return new UsersProjection(docs.GetWriter<unit, UserListView>());
        }
    }
}
