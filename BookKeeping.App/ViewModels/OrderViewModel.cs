using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BookKeeping.UI.ViewModels;
using System.Threading.Tasks;
using BookKeeping.Projections.ProductsList;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.App.ViewModels
{
    public class OrderViewModel : WorkspaceViewModel
    {
        readonly ObservableCollection<OrderLineViewModel> _lines = new ObservableCollection<OrderLineViewModel>();
        ObservableCollection<SimpleProductViewModel> _products = new ObservableCollection<SimpleProductViewModel>();
        private Projections.ProductsList.ProductListView _productListView;
        private readonly Session _session = Context.Current.GetSession();
        private OrderLineViewModel _selectedItem;

        public OrderViewModel()
        {
            DisplayName = T("SaleOfGoods");

            Lines.Add(new OrderLineViewModel
            {
                ItemNo = "item no. 1",
                Quantity = 2,
                Amount = new CurrencyAmount(10, Currency.Uah),
                Title = "title"
            });
            Lines.Add(new OrderLineViewModel
            {
                ItemNo = "item no. 2",
                Quantity = 4,
                Amount = new CurrencyAmount(20, Currency.Uah),
                Title = "title"
            });
            Lines.Add(new OrderLineViewModel
            {
                ItemNo = "item no. 3",
                Quantity = 6,
                Amount = new CurrencyAmount(30, Currency.Uah),
                Title = "title"
            });

            Task loadItemsTask = Task.Factory.StartNew(() =>
            {
                Products = new ObservableCollection<SimpleProductViewModel>(LoadItems());
            });
        }

        public virtual OrderLineViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                OnPropertyChanging(() => SelectedItem);
                if (_selectedItem != null)
                {
                    _selectedItem.IsSelected = false;
                }
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedItem.IsSelected = true;
                }
                OnPropertyChanged(() => SelectedItem);
            }
        }

        protected IEnumerable<SimpleProductViewModel> LoadItems()
        {
            _productListView = _session.Query<ProductListView>().Convert(t => t, new ProductListView());
            return GetProducts(_productListView);
        }

        protected virtual IEnumerable<SimpleProductViewModel> GetProducts(ProductListView view)
        {
            return view.Products.Select((p, i) => new SimpleProductViewModel
            {
                Id = p.Id.Id,
                ItemNo = p.ItemNo,
                Title = p.Title,
                IsValid = true
            });
        }

        public ObservableCollection<OrderLineViewModel> Lines { get { return _lines; } }

        public ObservableCollection<SimpleProductViewModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged(() => Products);
            }
        }
    }

    public sealed class SimpleProductViewModel : ViewModelBase
    {
        private long _id;

        public long Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        private string _itemNo;

        public string ItemNo
        {
            get { return _itemNo; }
            set
            {
                _itemNo = value;
                OnPropertyChanged(() => ItemNo);
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        public override string ToString()
        {
            return ItemNo + " " + Title;
        }
    }
}
