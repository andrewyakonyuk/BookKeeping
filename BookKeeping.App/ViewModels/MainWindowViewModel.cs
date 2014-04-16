using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Data;

namespace BookKeeping.App.ViewModels
{
    public class MainWindowViewModel : WorkspaceViewModel
    {
        private ObservableCollection<WorkspaceViewModel> _workspaces;

        public MainWindowViewModel()
        {
        }

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

        private TViewModel CreateOrExistWorkspace<TViewModel>()
            where TViewModel : WorkspaceViewModel, new()
        {
            TViewModel viewModel = Workspaces.OfType<TViewModel>().FirstOrDefault();
            if (viewModel == null)
            {
                viewModel = new TViewModel();
                Workspaces.Add(viewModel);
            }
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