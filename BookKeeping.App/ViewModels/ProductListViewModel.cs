using BookKeeping.Core;
using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookKeeping.Core.AtomicStorage;
using ICommand  = System.Windows.Input.ICommand;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections;

namespace BookKeeping.App.ViewModels
{
    public class ProductListViewModel : WorkspaceViewModel
    {
        string _searchText = string.Empty;
        bool _showFindPopup = false;
        bool _showProductDetail = false;
        ProductDto _selectedItem;
        IList<ProductDto> _selectedItems;

        public ProductListViewModel()
        {
            ShowProductDetail = true;

            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            EditProductCmd = new DelegateCommand(_ => ShowProductDetail = !ShowProductDetail, _ => SelectedItems.Count == 1);

            var reader = Context.Current.ViewDocs.GetReader<unit, ProductListDto>();
            var productList = reader.Get(unit.it);
            DisplayName = BookKeeping.App.Properties.Resources.Product_List;
            Source = productList.Convert(t => t.Products,
                () => new List<ProductDto>());
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

        public bool ShowFindPopup
        {
            get { return _showFindPopup; }
            set
            {
                OnPropertyChanging(() => ShowFindPopup);
                _showFindPopup = value;
                OnPropertyChanged(() => ShowFindPopup);
            }
        }

        public ProductDto SelectedItem
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
                _selectedItems = value.OfType<ProductDto>().ToList();
                OnPropertyChanged(() => SelectedItems);
            }
        }

        public ICommand SearchButtonCmd { get; private set; }

        public ICommand EditProductCmd { get; private set; }

        public ListCollectionView CollectionView { get { return (ListCollectionView)CollectionViewSource.GetDefaultView(Source); } }

        protected void DoSearch(string searchText)
        {
            CollectionView.Filter = (object t) =>
            {
                if (string.IsNullOrEmpty(searchText))
                    return true;
                var product = (ProductDto)t;
                return product.Title.StartsWith(searchText, StringComparison.CurrentCultureIgnoreCase);
            };
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
