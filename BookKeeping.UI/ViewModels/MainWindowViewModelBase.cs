using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace BookKeeping.UI.ViewModels
{
    public class MainWindowViewModelBase : WorkspaceViewModel, IObserver<MessageEnvelope>
    {
        private string _statusMessage;
        private ObservableCollection<WorkspaceViewModel> _workspaces;
        private WorkspaceViewModel _currentWorkspace;
        private readonly IList<ToolbarCommandViewModel> _toolbarCommands = new ObservableCollection<ToolbarCommandViewModel>();
        private readonly IList<MenuTreeNode> _mainMenuCommands = new ObservableCollection<MenuTreeNode>();

        protected readonly string ApplicationName = "";
        private readonly ICollectionView _workspacesCollectionView;

        public MainWindowViewModelBase()
        {
            ApplicationName = T("Application_Name");
            DisplayName = ApplicationName;

            StatusMessage = T("Application_Ready");

            BuildMainMenu();
            BuildToolbar();

            _workspacesCollectionView = CollectionViewSource.GetDefaultView(Workspaces);
            _workspacesCollectionView.CurrentChanged += _workspacesCollectionView_CurrentChanged;
        }

        public IList<ToolbarCommandViewModel> Toolbar { get { return _toolbarCommands; } }

        public IList<MenuTreeNode> MainMenu { get { return _mainMenuCommands; } }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (string.Equals(_statusMessage, value)) return;
                OnPropertyChanging(() => StatusMessage);
                _statusMessage = value;
                OnPropertyChanged(() => StatusMessage);
            }
        }

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

        public WorkspaceViewModel CurrentWorkspace
        {
            get { return _currentWorkspace; }
            set
            {
                _currentWorkspace = value;
                OnPropertyChanged(() => CurrentWorkspace);
            }
        }

        protected virtual void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                {
                    workspace.RequestClose += OnWorkspaceRequestClose;
                    var observable = workspace as IObservable<MessageEnvelope>;
                    if (observable != null)
                    {
                        observable.Subscribe(this);
                    }
                }
            if (e.OldItems == null || e.OldItems.Count == 0) return;
            foreach (WorkspaceViewModel workspace in e.OldItems)
            {
                workspace.RequestClose -= OnWorkspaceRequestClose;
                var observable = workspace as IObservable<MessageEnvelope>;
                if (observable != null && observable is IDisposable)
                {
                    ((IDisposable)observable).Dispose();
                }
            }
        }

        protected virtual void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            var workspace = sender as WorkspaceViewModel;
            if (workspace.CanClose)
            {
                Workspaces.Remove(workspace);
            }
        }

        protected virtual void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        private void _workspacesCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            var workspace = _workspacesCollectionView.CurrentItem as WorkspaceViewModel;
            CurrentWorkspace = workspace;

            if (workspace != null)
            {
                DisplayName = GetDisplayName(ApplicationName, workspace.DisplayName);
            }

            UpdateToolbar(workspace);
        }

        protected virtual void UpdateToolbar(WorkspaceViewModel workspace)
        {
            _toolbarCommands.Clear();
            if (workspace is IHasToolbarCommands)
            {
                foreach (var item in ((IHasToolbarCommands)workspace).Toolbar)
                {
                    _toolbarCommands.Add(item);
                }
            }
        }

        protected virtual TViewModel CreateOrRetrieveWorkspace<TViewModel>()
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

        protected virtual string GetLocalizationDisplayName(string prefix, string name)
        {
            return GetDisplayName(T(prefix), T(name));
        }

        protected virtual string GetDisplayName(string prefix, string name)
        {
            return string.Format("{0} - {1}", prefix, name);
        }

        protected virtual void BuildToolbar()
        {
        }

        protected virtual void BuildMainMenu()
        {
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
        }

        public virtual void OnNext(MessageEnvelope value)
        {
            StatusMessage = (string)value.Content;
        }
    }
}