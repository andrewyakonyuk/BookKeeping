using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BookKeeping.UI.ViewModels;
using System.Threading.Tasks;
using BookKeeping.Projections.ProductsList;
using BookKeeping.Domain.Contracts;
using ICommand = System.Windows.Input.ICommand;
using BookKeeping.UI;
using System.Collections.Specialized;
using MahApps.Metro.Controls.Dialogs;
using BookKeeping.Domain;

namespace BookKeeping.App.ViewModels
{
    public class OrderViewModel : WorkspaceViewModel
    {
        readonly ObservableCollection<OrderLineViewModel> _lines = new ObservableCollection<OrderLineViewModel>();
        ObservableCollection<SimpleProductViewModel> _products = new ObservableCollection<SimpleProductViewModel>();
        private Projections.ProductsList.ProductListView _productListView;
        private readonly ISession _session = Context.Current.GetSession();
        private OrderLineViewModel _selectedLine;

        public event EventHandler CheckoutCompleted = (sender, e) => { };

        public OrderViewModel()
        {
            DisplayName = T("SaleOfGoods");

            Currency = Domain.Contracts.Currency.Uah;

            Delivery = new CurrencyAmount(0, Currency);
            Discount = new CurrencyAmount(0, Currency);
            VatRate = new CurrencyAmount(0, Currency);
            TotalPrice = new CurrencyAmount(0, Currency);
            TotalPriceInclVat = new CurrencyAmount(0, Currency);

            AddLineCmd = new DelegateCommand(item => AddLine((SimpleProductViewModel)item, DefaultQuantity));
            RemoveLineCmd = new DelegateCommand(item => RemoveLine((OrderLineViewModel)item));
            CheckoutCmd = new DelegateCommand(_ => Checkout(), _ => CanCheckout);

            _lines.CollectionChanged += lines_CollectionChanged;

            Task loadItemsTask = Task.Factory.StartNew(() =>
            {
                Products = new ObservableCollection<SimpleProductViewModel>(LoadProducts());
            });

            this.PropertyChanged += OrderViewModel_PropertyChanged;

        }

        void OrderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != GetPropertyName(() => IsCheckoutCompleted))
            {
                IsCheckoutCompleted = false;
            }
        }

        void lines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Cast<OrderLineViewModel>().Any())
            {
                foreach (var item in e.NewItems.Cast<OrderLineViewModel>())
                {
                    Bind(item, t => t.Quantity, (s, args) => UpdateTotals());
                }
            }
            UpdateTotals();
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

        public virtual OrderLineViewModel SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                OnPropertyChanging(() => SelectedLine);
                if (_selectedLine != null)
                {
                    _selectedLine.IsSelected = false;
                }
                _selectedLine = value;
                if (_selectedLine != null)
                {
                    _selectedLine.IsSelected = true;
                }
                OnPropertyChanged(() => SelectedLine);
            }
        }

        private bool _isCheckoutCompleted;

        public bool IsCheckoutCompleted
        {
            get { return _isCheckoutCompleted; }
            set
            {
                _isCheckoutCompleted = value;
                OnPropertyChanged(() => IsCheckoutCompleted);
            }
        }
        

        private double _defaultQuantity = 1;

        public double DefaultQuantity
        {
            get { return _defaultQuantity; }
            set
            {
                _defaultQuantity = value;
                OnPropertyChanged(() => DefaultQuantity);
            }
        }

        private CurrencyAmount _totalPrice;

        public CurrencyAmount TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged(() => TotalPrice);
            }
        }

        private CurrencyAmount _totalPriceIncVat;

        public CurrencyAmount TotalPriceInclVat
        {
            get { return _totalPriceIncVat; }
            set
            {
                _totalPriceIncVat = value;
                OnPropertyChanged(() => TotalPriceInclVat);
            }
        }

        private CurrencyAmount _discount;

        public CurrencyAmount Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                OnPropertyChanged(() => Discount);
            }
        }

        private CurrencyAmount _delivery;

        public CurrencyAmount Delivery
        {
            get { return _delivery; }
            set
            {
                _delivery = value;
                OnPropertyChanged(() => Delivery);
            }
        }
        

        private CurrencyAmount _vatRate;

        public CurrencyAmount VatRate
        {
            get { return _vatRate; }
            set
            {
                _vatRate = value;
                OnPropertyChanged(() => VatRate);
            }
        }

        private double _totalQuantity;

        public double TotalQuantity
        {
            get { return _totalQuantity; }
            set
            {
                _totalQuantity = value;
                OnPropertyChanged(() => TotalQuantity);
            }
        }

        public Currency Currency { get; set; }

        public ICommand AddLineCmd { get; private set; }

        public ICommand RemoveLineCmd { get; private set; }

        public ICommand CheckoutCmd { get; private set; }

        public bool CanCheckout { get { return Lines.Any(); } }

        protected void RemoveLine(OrderLineViewModel line)
        {
            if (line == null)
                return;
            Lines.Remove(line);
        }

        protected void UpdateTotals()
        {
            TotalPrice = new CurrencyAmount(Lines.Aggregate(0M, (seed, line) => seed = seed + line.Amount.Amount * line.Quantity), Currency);
            TotalPriceInclVat = new CurrencyAmount(Lines.Aggregate(TotalPrice.Amount, (seed, line) => seed = seed + line.Amount.Amount * line.Quantity * line.VatRate.VatPersentage), Currency);
            VatRate = (TotalPriceInclVat - TotalPrice);
            TotalQuantity = _lines.Sum(t => (double)t.Quantity);
        }

        protected void AddLine(SimpleProductViewModel simpleProduct, double quantity)
        {
            if (simpleProduct == null)
                return;

            var product = _productListView.Products.SingleOrDefault(t => t.Id.Id == simpleProduct.Id);
            if (product == null)
                return;

            var line = Lines.SingleOrDefault(t => t.ProductId == simpleProduct.Id);
            if (line == null && quantity > 0)
            {
                Lines.Add(new OrderLineViewModel
                {
                    ProductId = product.Id.Id,
                    Amount = product.Price,
                    ItemNo = product.ItemNo,
                    Quantity = (decimal)quantity,
                    Title = product.Title,
                    VatRate = product.VatRate
                });
            }
            else
            {
                if (quantity <= 0)
                    return;
                line.Quantity += (decimal)quantity;
            }
        }

        protected void Checkout()
        {
            Lines.Clear();
            IsCheckoutCompleted = true;
            CheckoutCompleted(this, new EventArgs());
        }

        protected IEnumerable<SimpleProductViewModel> LoadProducts()
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
