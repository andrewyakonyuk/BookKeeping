using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows.Data;
using BookKeeping.Domain.Factories;
using BookKeeping.Domain.Services;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using ICommand = System.Windows.Input.ICommand;

namespace BookKeeping.App.ViewModels
{
    public class ProductListViewModel : WorkspaceViewModel
    {
        private string _searchText = string.Empty;
        private bool _showFindPopup = false;
        private bool _showFilterPopup = false;
        private object _selectedItem;
        private IList _selectedItems;
        private bool _hasChanges = false;
        private readonly ServiceFactory _serviceFactory;
        private string _filterText = string.Empty;
        private readonly Regex filterExpressionRegex = new Regex(@"^(\s*)(?<field>\w+)(\s*)(?<operator>\W+)(\s*)(?<value>.+)", RegexOptions.Compiled);
        private ProductViewModel _editingItem;
        private ProductViewModel _previousEditingItem;

        public ProductListViewModel()
        {
            _serviceFactory = new ServiceFactory();

            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            FilterButtonCmd = new DelegateCommand(_ => DoFilter(FilterText), _ => true);
            FilterPopupCmd = new DelegateCommand(_ => ShowFilterPopup = !ShowFilterPopup);
            EditProductCmd = new DelegateCommand(item => { EditingItem = item == EditingItem ? null : item as ProductViewModel; }, _ => SelectedItems.Count == 1);
            SaveCmd = new DelegateCommand(_ => SaveChanges(), _ => HasChanges && IsValid && CollectionView.OfType<ProductViewModel>().All(t => t.IsValid));

            DisplayName = BookKeeping.App.Properties.Resources.Product_List;

            var tempSource = new ObservableCollection<ProductViewModel>();
            Source = tempSource;
            tempSource.CollectionChanged += tempSource_CollectionChanged;
            foreach (var item in GetProducts())
            {
                tempSource.Add(item);
            }
            this.PropertyChanged += ProductListViewModel_PropertyChanged;
            HasChanges = false;
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                OnPropertyChanged(() => HasChanges);
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                OnPropertyChanging(() => SearchText);
                _searchText = value;
                OnPropertyChanged(() => SearchText);
            }
        }

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                OnPropertyChanged(() => FilterText);
            }
        }

        public bool ShowFindPopup
        {
            get { return _showFindPopup; }
            set
            {
                if (value)
                    ShowFilterPopup = false;
                else
                {
                    SearchText = string.Empty;
                }
                OnPropertyChanging(() => ShowFindPopup);
                _showFindPopup = value;
                OnPropertyChanged(() => ShowFindPopup);
            }
        }

        public bool ShowFilterPopup
        {
            get { return _showFilterPopup; }
            set
            {
                if (value)
                    ShowFindPopup = false;
                else
                {
                    ResetFilter();
                }
                OnPropertyChanging(() => ShowFilterPopup);
                _showFilterPopup = value;
                OnPropertyChanged(() => ShowFilterPopup);
            }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                OnPropertyChanging(() => SelectedItem);
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }

        public IList SelectedItems
        {
            get { return (IList)_selectedItems; }
            set
            {
                OnPropertyChanging(() => SelectedItems);
                _selectedItems = value;
                OnPropertyChanged(() => SelectedItems);
            }
        }

        public ProductViewModel EditingItem
        {
            get { return _editingItem; }
            set
            {
                _previousEditingItem = _editingItem;
                _editingItem = value;

                if (_editingItem != null)
                    _editingItem.IsEdit = true;

                OnPropertyChanged(() => EditingItem);

                if (_previousEditingItem != null)
                    _previousEditingItem.IsEdit = false;
            }
        }

        public ICommand SearchButtonCmd { get; private set; }

        public ICommand FilterButtonCmd { get; private set; }

        public ICommand EditProductCmd { get; private set; }

        public ICommand FilterPopupCmd { get; private set; }

        public ICommand SaveCmd { get; private set; }

        public ListCollectionView CollectionView { get { return (ListCollectionView)CollectionViewSource.GetDefaultView(Source); } }

        protected void SaveChanges()
        {
            HasChanges = false;
            foreach (var item in Source as IEnumerable<ProductViewModel>)
            {
                item.HasChanges = false;
            }
        }

        protected virtual IEnumerable<ProductViewModel> GetProducts()
        {
            return _serviceFactory.Create<IProductService>().GetAll().Select((p, i) => new ProductViewModel
            {
                Barcode = p.Barcode,
                IsOrderable = p.IsOrderable,
                ItemNo = p.ItemNo,
                Price = p.Price,
                Stock = p.Stock,
                Title = p.Title,
                UnitOfMeasure = p.UnitOfMeasure,
                VatRate = p.VatRate,
                HasChanges = false,
                IsValid = true
            });
        }

        protected void DoSearch(string searchText)
        {
            CollectionView.Filter = (object t) =>
            {
                var product = t as ProductViewModel;
                if (string.IsNullOrEmpty(searchText))
                    return true;
                return product.ItemNo.IndexOf(searchText) > -1;
            };
        }

        protected void DoFilter(string filterExpression)
        {
            Func<ProductViewModel, bool> selector = (p) => false;
            try
            {
                if (!string.IsNullOrWhiteSpace(filterExpression) && filterExpressionRegex.IsMatch(filterExpression))
                {
                    var groups = filterExpressionRegex.Match(filterExpression).Groups;
                    selector = CreateFilterFunc<ProductViewModel>(groups["field"].Value.Trim(),
                        groups["operator"].Value.Trim(),
                        groups["value"].Value.Trim());
                }
            }
            catch (NotSupportedException) { }
            catch (FormatException) { }
            catch (InvalidOperationException) { }
            finally
            {
                foreach (var item in ((IEnumerable<ProductViewModel>)Source))
                {
                    if (selector(item))
                        item.IsHighlight = true;
                    else item.IsHighlight = false;
                }
            }
        }

        protected void ResetFilter()
        {
            FilterText = string.Empty;
            foreach (var item in ((IEnumerable<ProductViewModel>)Source))
            {
                item.IsHighlight = false;
            }
        }

        protected Func<T, bool> CreateFilterFunc<T>(string fieldStr, string operatorStr, string valueStr)
        {
            var parameter = Expression.Parameter(typeof(T));
            var field = Expression.Property(parameter, fieldStr);
            Expression value = null;
            decimal tempDecimal;
            bool tempBoolean;
            if (decimal.TryParse(valueStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out tempDecimal))
            {
                value = Expression.Constant(tempDecimal);
            }
            else if (bool.TryParse(valueStr, out tempBoolean))
            {
                value = Expression.Constant(tempBoolean);
            }
            else value = Expression.Constant(valueStr);

            Expression @operator = null;
            switch (operatorStr)
            {
                case "<":
                    @operator = Expression.LessThan(field, value);
                    break;

                case "<=":
                    @operator = Expression.LessThanOrEqual(field, value);
                    break;

                case "=":
                    @operator = Expression.Equal(field, value);
                    break;

                case ">":
                    @operator = Expression.GreaterThan(field, value);
                    break;

                case ">=":
                    @operator = Expression.GreaterThanOrEqual(field, value);
                    break;
                    throw new NotSupportedException();
            }
            return Expression.Lambda<Func<T, bool>>(@operator, parameter).Compile();
        }

        private void tempSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    HasChanges = true;
                    break;

                default:
                    break;
            }

            if (e.OldItems != null)
            {
                foreach (ProductViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (ProductViewModel item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChanges")
            {
                var item = (ProductViewModel)sender;
                if (item.HasChanges)
                    this.HasChanges = true;
            }
        }

        private void ProductListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetPropertyName(() => HasChanges))
            {
                var end = " *";
                if (HasChanges && !DisplayName.EndsWith(end))
                {
                    DisplayName = DisplayName + end;
                }
                else if (!HasChanges && DisplayName.EndsWith(end))
                {
                    DisplayName = DisplayName.Substring(0, DisplayName.Length - 2);
                }
            }
        }
    }
}