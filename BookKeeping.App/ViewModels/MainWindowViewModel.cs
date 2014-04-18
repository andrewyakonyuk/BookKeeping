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

namespace BookKeeping.App.ViewModels
{
    public class MainWindowViewModel : WorkspaceViewModel
    {
        private ObservableCollection<WorkspaceViewModel> _workspaces;

        public MainWindowViewModel()
        {
            Workspaces.Add(new WorkspaceViewModel
            {
                DisplayName = "Some workspace"
            });

            OpenCustomerTransactions = new DelegateCommand(_ =>
            {
                bool isExist;
                var viewModel = CreateOrGetExistWorkspace<CustomerTransactionsViewModel>(out isExist);
                if (!isExist)
                {
                    var reader = Context.Current.ViewDocs.GetReader<CustomerId, CustomerTransactionsDto>();
                    var transactions = reader.Get(new CustomerId(12));
                    viewModel.DisplayName = "Customer transitions";
                    viewModel.Source = Source = transactions.Convert(t => t.Transactions, () => new List<CustomerTransactionDto>());
                    SetActiveWorkspace(viewModel);
                }
            });
        }

        public ICommand OpenCustomerTransactions { get; set; }

        public ICommand CloseTabItem { get; set; }

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