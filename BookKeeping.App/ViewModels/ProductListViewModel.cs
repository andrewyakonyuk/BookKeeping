using BookKeeping.App.Helpers;
using BookKeeping.Domain.Contracts;
using BookKeeping.Projections.ProductsList;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using ICommand = System.Windows.Input.ICommand;

namespace BookKeeping.App.ViewModels
{
    public class ProductListViewModel : WorkspaceViewModel, IPrintable, ISaveable
    {
        private string _searchText = string.Empty;
        private bool _showFindPopup = false;
        private bool _showFilterPopup = false;
        private object _selectedItem;
        private IList _selectedItems;
        private bool _hasChanges = false;
        private string _filterText = string.Empty;
        private ProductViewModel _editingItem;
        private ProductViewModel _previousEditingItem;
        private readonly ExpressionHelper _expressionHelper = new ExpressionHelper();

        public ProductListViewModel()
        {
            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            FilterButtonCmd = new DelegateCommand(_ => DoFilter(FilterText), _ => true);
            FilterPopupCmd = new DelegateCommand(_ => ShowFilterPopup = !ShowFilterPopup);
            EditProductCmd = new DelegateCommand(item => { EditingItem = item == EditingItem ? null : item as ProductViewModel; }, _ => SelectedItems.Count == 1);
            SaveCmd = new DelegateCommand(_ => SaveChanges(), _ => CanSave);

            DisplayName = T("ListOfProducts");

            var tempSource = new ObservableCollection<ProductViewModel>();
            Source = tempSource;
            tempSource.CollectionChanged += tempSource_CollectionChanged;
            foreach (var item in GetProducts())
            {
                tempSource.Add(item);
            }
            Bind(() => HasChanges, HasChangesPropertyChanged);
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

        public DataGrid ProductListTable { get; set; }

        protected virtual IEnumerable<ProductViewModel> GetProducts()
        {
            var random = new Random(100);
            return GetProductListProjection().Select((p, i) => new ProductViewModel
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

        private IEnumerable<ProductView> GetProductListProjection()
        {
            var random = new Random(100);
            for (int i = 0; i < 500; i++)
            {
                yield return new ProductView
                   {
                       Id = new ProductId(i),
                       Barcode = new Barcode("12342323", BarcodeType.EAN13),
                       IsOrderable = true,
                       ItemNo = "item no. " + (i + 1),
                       Price = new CurrencyAmount(random.Next(10, 100), Currency.Eur),
                       Stock = random.Next(1, 1000),
                       Title = new string("qwertyuiopasdfghjklzxcvbnm".Substring(random.Next(0, 12)).OrderBy(t => Guid.NewGuid()).ToArray()),
                       UnitOfMeasure = "m2",
                       VatRate = new VatRate(new decimal(random.NextDouble())),
                   };
            }
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
                var filterSelector = _expressionHelper.GetFilter<ProductViewModel>(filterExpression);
                if (filterExpression != null)
                    selector = filterSelector;
            }
            catch (Exception ex)
            {
                //todo: logging
            }
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
                    Unbind(item, t => t.HasChanges, ProductViewModel_HasChangesPropertyChanged);
                }
            }
            if (e.NewItems != null)
            {
                foreach (ProductViewModel item in e.NewItems)
                {
                    Bind(item, t => t.HasChanges, ProductViewModel_HasChangesPropertyChanged);
                }
            }
        }

        private void ProductViewModel_HasChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (ProductViewModel)sender;
            if (item.HasChanges)
                this.HasChanges = true;
        }

        private void HasChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var end = " *";
            if (HasChanges)
            {
                CanClose = false;
            }
            if (HasChanges && !DisplayName.EndsWith(end))
            {
                DisplayName = DisplayName + end;
            }
            else if (!HasChanges && DisplayName.EndsWith(end))
            {
                DisplayName = DisplayName.Substring(0, DisplayName.Length - 2);
            }
        }

        public void Print()
        {
            SendMessage(new MessageEnvelope(T("PrintingNotCompleted")));
            //var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //bool? result = saveFileDialog.ShowDialog();
            //if (result == true)
            //{
            //    App.Current.Dispatcher.BeginInvoke((Action)delegate
            //    {
            //        var exporter = new PdfExporter();
            //        var documentPaginator = new DataGridDocumentPaginator(ProductListTable, string.Empty, new System.Windows.Size(940, 1070), new System.Windows.Thickness());
            //        exporter.Export(documentPaginator, saveFileDialog.FileName);
            //        SendMessage(new MessageEnvelope(T("PrintCompleted")));
            //    });
            //}
        }

        public void SaveChanges()
        {
            if (CanSave)
            {
                HasChanges = false;
                foreach (var item in Source as IEnumerable<ProductViewModel>)
                {
                    item.HasChanges = false;
                }
            }
        }

        public bool CanSave
        {
            get
            {
                return HasChanges && IsValid && CollectionView.OfType<ProductViewModel>().All(t => t.IsValid);
            }
        }
    }
}