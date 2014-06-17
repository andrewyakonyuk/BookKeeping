using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using System.Linq;

namespace BookKeeping.Projections.VendorList
{
    public class VendorsProjection :
        IEventHandler<VendorCreated>,
        IEventHandler<VendorDeleted>,
        IEventHandler<VendorInfoUpdated>,
        IEventHandler<VendorAddressUpdated>
    {
        private readonly IDocumentWriter<unit, VendorListView> _store;

        public VendorsProjection(IDocumentWriter<unit, VendorListView> store)
        {
            _store = store;
        }

        public void When(VendorCreated e)
        {
            _store.UpdateEnforcingNew(unit.it, prs => prs.Vendors.Add(
            new VendorView
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

        public void When(VendorDeleted e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Vendors.Remove(list.Vendors.Find(t => t.Id == e.Id)));
        }

        public void When(VendorAddressUpdated e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                var item = list.Vendors.Find(t => t.Id == e.Id);
                item.LegalAddress = e.Address;
            });
        }

        public void When(VendorInfoUpdated e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                var item = list.Vendors.Find(t => t.Id == e.Id);
                item.BankingDetails = e.BankingDetails;
                item.Email = e.Email;
                item.Fax = e.Fax;
                item.Phone = e.Phone;
            });
        }
    }
}
