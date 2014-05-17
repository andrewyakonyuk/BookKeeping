using BookKeeping.App.Common;
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

namespace BookKeeping.App.ViewModels
{
    public class ProductListViewModel : WorkspaceViewModel
    {
        string _searchText = string.Empty;
        bool _showFindPopup = false;
        bool _showProductDetail = false;
        object _selectedItem;
        IList _selectedItems;
        readonly ServiceFactory _serviceFactory;

        public ProductListViewModel()
        {
            _serviceFactory = new ServiceFactory();

            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            EditProductCmd = new DelegateCommand(_ => ShowProductDetail = !ShowProductDetail, _ => SelectedItems.Count == 1);

            DisplayName = BookKeeping.App.Properties.Resources.Product_List;

            Source = new ObservableCollection<Product>(_serviceFactory.Create<IProductService>().GetAll());
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
                if (!value && CollectionView.IsEditingItem)
                {
                    CollectionView.CommitEdit();
                    if (CollectionView.NeedsRefresh)
                        CollectionView.Refresh();
                }
                OnPropertyChanging(() => ShowFindPopup);
                _showFindPopup = value;
                OnPropertyChanged(() => ShowFindPopup);
            }
        }

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

        public ICommand EditProductCmd { get; private set; }

        public ListCollectionView CollectionView { get { return (ListCollectionView)CollectionViewSource.GetDefaultView(Source); } }

        protected void DoSearch(string searchText)
        {
            CollectionView.Filter = (object t) =>
            {
                if (string.IsNullOrEmpty(searchText))
                    return true;
                return true;
                //TODO: search
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
