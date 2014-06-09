using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class CustomerListViewModel : ListViewModel<CustomerViewModel>
    {
        public CustomerListViewModel()
        {
            DisplayName = T("ListOfCustomers");
        }

        protected override IEnumerable<CustomerViewModel> LoadItems()
        {
            return Enumerable.Empty<CustomerViewModel>();
        }
    }
}
