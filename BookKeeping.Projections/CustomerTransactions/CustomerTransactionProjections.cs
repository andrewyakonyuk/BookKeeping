using BookKeeping.Core.AtomicStorage;
using BookKeeping.Core.Domain;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Projections.CustomerTransactions
{
    public class CustomerTransactionsProjection : 
        IEventHandler<CustomerCreated>,
        IEventHandler<CustomerChargeAdded>,
        IEventHandler<CustomerPaymentAdded>
    {
        private readonly IDocumentWriter<CustomerId, CustomerTransactionsListView> _store;

        public CustomerTransactionsProjection(IDocumentWriter<CustomerId, CustomerTransactionsListView> store)
        {
            _store = store;
        }

        public void When(CustomerCreated e)
        {
            _store.Add(e.Id, new CustomerTransactionsListView());
        }

        public void When(CustomerChargeAdded e)
        {
            _store.UpdateOrThrow(e.Id, v => v.AddTx(e.ChargeName, -e.Charge, e.NewBalance, e.TimeUtc));
        }

        public void When(CustomerPaymentAdded e)
        {
            _store.UpdateOrThrow(e.Id, v => v.AddTx(e.PaymentName, e.Payment, e.NewBalance, e.TimeUtc));
        }
    }
}