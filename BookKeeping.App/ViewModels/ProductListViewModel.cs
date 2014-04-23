﻿using BookKeeping.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICommand  = System.Windows.Input.ICommand;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            EditProductCmd = new DelegateCommand(_ => ShowProductDetail = !ShowProductDetail, _ => SelectedItems.Count == 1);

            DisplayName = BookKeeping.App.Properties.Resources.Product_List;

           // var reader = Context.Current.ViewDocs.GetReader<unit, ProductListDto>();

            //var productList = new ObservableCollection<ProductDto>((reader.Get(unit.it).Convert(t => t.Products,
            //    () => new List<ProductDto>())));
            var productList = new ObservableCollection<ProductDto>();
            productList.CollectionChanged += productList_CollectionChanged;
            Source = productList;
        }


        void productList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

        public ProductDto SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (ShowProductDetail)
                {
                    var currentItem = CollectionView.CurrentItem;
                }
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
