using BookKeeping.Domain;
using BookKeeping.Projections;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Data;
using BookKeeping.Core.AtomicStorage;
using System.Collections.Generic;
using System.Windows.Input;
using BookKeeping.App.Views;
using BookKeeping.Core;
using ICommand = System.Windows.Input.ICommand;

namespace BookKeeping.App.ViewModels
{
    public class MainWindowViewModel : WorkspaceViewModel
    {
        private ObservableCollection<WorkspaceViewModel> _workspaces;

        public MainWindowViewModel()
        {
            this.DisplayName = BookKeeping.App.Properties.Resources.MainWindow_Title;

            CollectionViewSource.GetDefaultView(Workspaces).CurrentChanged += MainWindowViewModel_CurrentChanged;

            Workspaces.Add(new WorkspaceViewModel
            {
                DisplayName = "Empty workspace"
            });

            OpenCustomerTransactions = new DelegateCommand(_ =>
            {
                bool isExist;
                var viewModel = CreateOrGetExistWorkspace<CustomerTransactionsViewModel>(out isExist);
                SetActiveWorkspace(viewModel);
            });

            OpenProductList = new DelegateCommand(_ =>
            {
                bool isExist;
                var viewModel = CreateOrGetExistWorkspace<ProductListViewModel>(out isExist);
                SetActiveWorkspace(viewModel);
            });

            //int counter = 1;
            //AddProduct = new DelegateCommand(_ =>
            //{
            //    Context.Current.CommandBus.Send(new CreateProduct
            //    {
            //        Id = new ProductId(Guid.NewGuid()),
            //        Title = "Some title" + counter,
            //        ItemNo = string.Format("art {0:00000}", counter),
            //        Price = new CurrencyAmount(counter * 14, Currency.Eur),
            //        Stock = 34 * counter - 15 / counter
            //    });
            //    var workspacesView = CollectionViewSource.GetDefaultView(Workspaces);
            //    var collectionView =  (ListCollectionView)CollectionViewSource.GetDefaultView(((ViewModelBase)workspacesView.CurrentItem).Source);
            //     var reader = Context.Current.ViewDocs.GetReader<unit, ProductListDto>();
            //        var productList = reader.Get(unit.it);
            //        collectionView.AddNewItem(productList.Convert(t => t.Products,
            //                () => new List<ProductDto>()).LastOrDefault());
            //        collectionView.MoveCurrentToLast();
            //    counter++;
            //}, _ =>
            //{
            //    var collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            //    return collectionView.CurrentItem is ProductListViewModel;
            //});

            Exit = ApplicationCommands.Close;
        }

        public ICommand AddProduct { get; set; }

        public ICommand OpenCustomerTransactions { get; set; }

        public ICommand OpenProductList { get; set; }

        public ICommand CloseTabItem { get; set; }

        public ICommand Exit { get; set; }

        #region Workspaces

        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                {
                    if (_workspaces == null)
                    {
                        _workspaces = new ObservableCollection<WorkspaceViewModel>();
                        _workspaces.CollectionChanged += OnWorkspacesChanged;
                    }
                    return _workspaces;
                }
            }
        }

        public void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Contract.Requires(Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        void MainWindowViewModel_CurrentChanged(object sender, EventArgs e)
        {
            var workspace = (WorkspaceViewModel)CollectionViewSource.GetDefaultView(Workspaces).CurrentItem;
            if (workspace == null)
                return;
            DisplayName = string.Format("{0} - {1}", BookKeeping.App.Properties.Resources.MainWindow_Title, workspace.DisplayName); ;
        }

        private TViewModel CreateOrGetExistWorkspace<TViewModel>(out bool isExist)
            where TViewModel : WorkspaceViewModel, new()
        {
            TViewModel viewModel = Workspaces.OfType<TViewModel>().FirstOrDefault();
            if (viewModel == null)
            {
                viewModel = new TViewModel();
                Workspaces.Add(viewModel);
                isExist = false;
                return viewModel;
            }
            isExist = true;
            return viewModel;
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            Workspaces.Remove(sender as WorkspaceViewModel);
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += OnWorkspaceRequestClose;

            if (e.OldItems == null || e.OldItems.Count == 0) return;
            foreach (WorkspaceViewModel workspace in e.OldItems)
                workspace.RequestClose -= OnWorkspaceRequestClose;
        }

        #endregion Workspaces
    }
}