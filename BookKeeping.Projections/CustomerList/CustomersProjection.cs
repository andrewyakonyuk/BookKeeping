using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using BookKeeping.Persistent;
using BookKeeping.Persistent.AtomicStorage;
using System.Linq;

namespace BookKeeping.Projections.CustomerList
{
    public class CustomersProjection :
        IEventHandler<CustomerCreated>,
        IEventHandler<CustomerDeleted>,
        IEventHandler<CustomerInfoUpdated>,
        IEventHandler<CustomerAddressUpdated>
    {
        private readonly IDocumentWriter<unit, CustomerListView> _store;

        public CustomersProjection(IDocumentWriter<unit, CustomerListView> store)
        {
            _store = store;
        }

        public void When(CustomerCreated e)
        {
            _store.UpdateEnforcingNew(unit.it, prs => prs.Customers.Add(
            new CustomerView
            {
                Id = e.Id,
                Balance = new CurrencyAmount(0, e.Currency),
                Currency = e.Currency,
                LegalAddress = new Address(),
                BankingDetails = string.Empty,
                Email = string.Empty,
                Fax = string.Empty,
                ManualBilling = false,
                Name = e.Name,
                Phone = string.Empty
            }));
        }

        public void When(CustomerDeleted e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Customers.Remove(list.Customers.Find(t => t.Id == e.Id)));
        }

        public void When(CustomerAddressUpdated e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                var item = list.Customers.Find(t => t.Id == e.Id);
                item.LegalAddress = e.Address;
            });
        }

        public void When(CustomerInfoUpdated e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                var item = list.Customers.Find(t => t.Id == e.Id);
                item.BankingDetails = e.BankingDetails;
                item.Email = e.Email;
                item.Fax = e.Fax;
                item.Phone = e.Phone;
            });
        }
    }
}
