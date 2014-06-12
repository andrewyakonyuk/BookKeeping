using BookKeeping.Domain.Contracts;
using BookKeeping.Projections.CustomerList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class CustomerListViewModel : ListViewModel<CustomerViewModel>
    {
        private Projections.CustomerList.CustomerListView _customerListView;
        private Session _session = Context.Current.GetSession();

        public CustomerListViewModel()
        {
            DisplayName = T("ListOfCustomers");
        }

        protected override IEnumerable<CustomerViewModel> LoadItems()
        {
            _customerListView = _session.Query<CustomerListView>().Convert(t => t, new CustomerListView());
            return GetCustomers(_customerListView);
        }

        protected virtual IEnumerable<CustomerViewModel> GetCustomers(CustomerListView view)
        {
            return view.Customers.Select((c, i) => new CustomerViewModel
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

        protected override void SaveNewItems(IEnumerable<CustomerViewModel> newItems)
        {
            foreach (var item in newItems)
            {
                var customer = _customerListView.Customers.Find(t => t.Id == new CustomerId(item.Id));
                if (customer == null)
                {
                    var id = _session.GetId();
                    _session.Command(new CreateCustomer(new CustomerId(id), item.FullName, Currency.Uah));
                    _session.Command(new UpdateCustomerAddress(new CustomerId(id), new Address
                    {
                        City = item.LegalAddress.City,
                        Country = item.LegalAddress.Country,
                        Street = item.LegalAddress.Street,
                        ZipCode = item.LegalAddress.ZipCode
                    }));
                    _session.Command(new UpdateCustomerInfo(new CustomerId(id), string.Empty, item.Phone, item.Fax, item.Email));
                    item.Id = id;
                }
            }
        }

        protected override void SaveUpdatedItems(IEnumerable<CustomerViewModel> updatesItems)
        {
            foreach (var item in updatesItems)
            {
                var customer = _customerListView.Customers.Find(t => t.Id == new CustomerId(item.Id));
                if (customer.Name != item.FullName)
                {
                    _session.Command(new RenameCustomer(customer.Id, item.FullName));
                }
                if (item.LegalAddress.Country != customer.LegalAddress.Country
                    || item.LegalAddress.City != customer.LegalAddress.City
                    || item.LegalAddress.Street != customer.LegalAddress.Street
                    || item.LegalAddress.ZipCode != customer.LegalAddress.ZipCode)
                {
                    _session.Command(new UpdateCustomerAddress(customer.Id, new Address
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
                    _session.Command(new UpdateCustomerInfo(customer.Id, string.Empty, item.Phone, item.Fax, item.Email));
                }
            }
        }

        protected override void SaveDeletedItems(IEnumerable<CustomerViewModel> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                _session.Command(new DeleteCustomer(new CustomerId(item.Id)));
            }
        }
    }
}
