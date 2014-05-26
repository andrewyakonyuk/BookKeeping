using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class BasketViewModel : WorkspaceViewModel
    {
        readonly ObservableCollection<BasketLineViewModel> _lines = new ObservableCollection<BasketLineViewModel>();

        public BasketViewModel()
        {

        }

        public ICollection<BasketLineViewModel> Lines { get { return _lines; } }
    }
}
