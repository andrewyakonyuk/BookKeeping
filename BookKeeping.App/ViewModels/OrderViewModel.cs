using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class OrderViewModel : WorkspaceViewModel
    {
        readonly ObservableCollection<OrderLineViewModel> _lines = new ObservableCollection<OrderLineViewModel>();

        public OrderViewModel()
        {
            DisplayName = T("Order");
        }

        public ICollection<OrderLineViewModel> Lines { get { return _lines; } }
    }
}
