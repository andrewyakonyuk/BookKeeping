using BookKeeping.Domain.Contracts;
using BookKeeping.Projections.VendorList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class VendorListViewModel : ListViewModel<VendorViewModel>
    {
        private Projections.VendorList.VendorListView _vendorListView;
        private Session _session = Context.Current.GetSession();

        public VendorListViewModel()
        {
            DisplayName = T("VendorList");
        }

        protected override IEnumerable<VendorViewModel> LoadItems()
        {
            _vendorListView = _session.Query<VendorListView>().Convert(t => t, new VendorListView());
            return GetVendors(_vendorListView);
        }

        protected virtual IEnumerable<VendorViewModel> GetVendors(VendorListView view)
        {
            return view.Vendors.Select((c, i) => new VendorViewModel
            {
                Id = c.Id.Id,
                Email = c.Email,
                Fax = c.Fax,
                FullName = c.Name,
                Phone = c.Phone,
                LegalAddress = new AddressViewModel
                {
                    City = c.LegalAddress.City,
                    Country = c.LegalAddress.Country,
                    Street = c.LegalAddress.Street,
                    ZipCode = c.LegalAddress.ZipCode
                },
                HasChanges = false,
                IsValid = true
            });
        }

        protected override void CommitChanges()
        {
            base.CommitChanges();
            _session.Commit();
        }

        protected override void SaveNewItems(IEnumerable<VendorViewModel> newItems)
        {
            foreach (var item in newItems)
            {
                var customer = _vendorListView.Vendors.Find(t => t.Id == new VendorId(item.Id));
                if (customer == null)
                {
                    var id = _session.GetId();
                    _session.Command(new CreateVendor(new VendorId(id), item.FullName, Currency.Uah));
                    _session.Command(new UpdateVendorAddress(new VendorId(id), new Address
                    {
                        City = item.LegalAddress.City,
                        Country = item.LegalAddress.Country,
                        Street = item.LegalAddress.Street,
                        ZipCode = item.LegalAddress.ZipCode
                    }));
                    _session.Command(new UpdateVendorInfo(new VendorId(id), string.Empty, item.Phone, item.Fax, item.Email));
                    item.Id = id;
                }
            }
        }

        protected override void SaveUpdatedItems(IEnumerable<VendorViewModel> updatesItems)
        {
            foreach (var item in updatesItems)
            {
                var customer = _vendorListView.Vendors.Find(t => t.Id == new VendorId(item.Id));
                if (customer.Name != item.FullName)
                {
                    _session.Command(new RenameVendor(customer.Id, item.FullName));
                }
                if (item.LegalAddress.Country != customer.LegalAddress.Country
                    || item.LegalAddress.City != customer.LegalAddress.City
                    || item.LegalAddress.Street != customer.LegalAddress.Street
                    || item.LegalAddress.ZipCode != customer.LegalAddress.ZipCode)
                {
                    _session.Command(new UpdateVendorAddress(customer.Id, new Address
                    {
                        City = item.LegalAddress.City,
                        Country = item.LegalAddress.Country,
                        Street = item.LegalAddress.Street,
                        ZipCode = item.LegalAddress.ZipCode
                    }));
                }
                if (item.Phone != customer.Phone
                    || item.Fax != customer.Fax
                    || item.Email != customer.Email)
                {
                    _session.Command(new UpdateVendorInfo(customer.Id, string.Empty, item.Phone, item.Fax, item.Email));
                }
            }
        }

        protected override void SaveDeletedItems(IEnumerable<VendorViewModel> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                _session.Command(new DeleteVendor(new VendorId(item.Id)));
            }
        }
    }
}
