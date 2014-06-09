using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class VendorListViewModel : ListViewModel<VendorViewModel>
    {
        public VendorListViewModel()
        {
            DisplayName = T("VendorList");
        }

        protected override IEnumerable<VendorViewModel> LoadItems()
        {
            return Enumerable.Empty<VendorViewModel>();
        }
    }
}
