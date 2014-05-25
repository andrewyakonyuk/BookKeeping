using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using ICommand = System.Windows.Input.ICommand;
using BookKeeping.Domain;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Domain.Factories;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace BookKeeping.App.ViewModels
{
    public class ProductListViewModel : WorkspaceViewModel
    {
        string _searchText = string.Empty;
        bool _showFindPopup = false;
        bool _showFilterPopup = false;
        bool _showProductDetail = false;
        object _selectedItem;
        IList _selectedItems;
        readonly ServiceFactory _serviceFactory;
        readonly Regex filterExpressionRegex = new Regex(@"^(\s*)(?<field>\w+)(\s*)(?<operator>\W+)(\s*)(?<value>.+)", RegexOptions.Compiled);

        public ProductListViewModel()
        {
            _serviceFactory = new ServiceFactory();

            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            FilterButtonCmd = new DelegateCommand(_ => DoFilter(FilterText), _ => true);
            FilterPopupCmd = new DelegateCommand(_ => ShowFilterPopup = !ShowFilterPopup);
            EditProductCmd = new DelegateCommand(_ => ShowProductDetail = !ShowProductDetail, _ => SelectedItems.Count == 1);

            DisplayName = BookKeeping.App.Properties.Resources.Product_List;

            Source = new ObservableCollection<ProductViewModel>(GetProducts());

            this.PropertyChanged += ProductListViewModel_PropertyChanged;
        }

        void ProductListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case "SearchText":
            //        SearchButtonCmd.Execute(null);
            //        break;
            //    default:
            //        break;
            //}
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
                     HasChanges = false
                 });
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

        private string _filterText;

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
                if (!value && CollectionView.IsEditingItem)
                {
                    CollectionView.CommitEdit();
                    if (CollectionView.NeedsRefresh)
                        CollectionView.Refresh();
                }
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

        public ICommand FilterPopupCmd { get; private set; }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                ShowProductDetail = false;
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

        public ICommand SearchButtonCmd { get; private set; }

        public ICommand FilterButtonCmd { get; private set; }

        public ICommand EditProductCmd { get; private set; }

        public ListCollectionView CollectionView { get { return (ListCollectionView)CollectionViewSource.GetDefaultView(Source); } }

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

        public bool ShowProductDetail
        {
            get { return _showProductDetail; }
            set
            {
                OnPropertyChanging(() => ShowProductDetail);
                _showProductDetail = value;
                OnPropertyChanged(() => ShowProductDetail);
            }
        }

    }
}
