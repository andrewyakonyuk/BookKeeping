using BookKeeping.App.Helpers;
using BookKeeping.Domain.Contracts;
using BookKeeping.Projections.UserList;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ICommand = System.Windows.Input.ICommand;

namespace BookKeeping.App.ViewModels
{
    public class UserListViewModel : WorkspaceViewModel, ISaveable
    {
        private string _searchText = string.Empty;
        private bool _isFindPopupVisible = false;
        private bool _isFilterPopupVisible = false;
        private object _selectedItem;
        private IList _selectedItems;
        private bool _hasChanges = false;
        private string _filterText = string.Empty;
        private UserViewModel _editingItem;
        private UserViewModel _previousEditingItem;
        private readonly ExpressionHelper _expressionHelper = new ExpressionHelper();
        private bool _isLoading;
        private Session _session = Context.Current.GetSession();
        private Projections.UserList.UserListView _userListView;
        private readonly List<UserViewModel> _changedProducts = new List<UserViewModel>();

        public UserListViewModel()
        {
            SearchButtonCmd = new DelegateCommand(_ => DoSearch(SearchText), _ => true);
            FilterButtonCmd = new DelegateCommand(_ => DoFilter(FilterText), _ => true);
            FilterPopupCmd = new DelegateCommand(_ => IsFilterPopupVisible = !IsFilterPopupVisible);
            EditProductCmd = new DelegateCommand(item => { EditingItem = item == EditingItem ? null : item as UserViewModel; }, _ => SelectedItems.Count == 1);
            SaveCmd = new DelegateCommand(_ => SaveChanges(), _ => CanSave);

            DisplayName = T("ListOfUsers");
            IsLoading = true;

            var tempSource = new ObservableCollection<UserViewModel>();

            tempSource.CollectionChanged += tempSource_CollectionChanged;
            Bind(() => HasChanges, HasChangesPropertyChanged);

            Task loadProductsTask = Task.Factory.StartNew(() =>
            {
                _userListView = _session.Query<UserListView>().Convert(t => t, new UserListView());
                foreach (var item in GetUsers(_userListView))
                {
                    tempSource.Add(item);
                }
                Source = tempSource;
                HasChanges = false;
                IsLoading = false;
            });
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
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

        public bool IsFindPopupVisible
        {
            get { return _isFindPopupVisible; }
            set
            {
                if (value)
                    IsFilterPopupVisible = false;
                else
                {
                    SearchText = string.Empty;
                }
                OnPropertyChanging(() => IsFindPopupVisible);
                _isFindPopupVisible = value;
                OnPropertyChanged(() => IsFindPopupVisible);
            }
        }

        public bool IsFilterPopupVisible
        {
            get { return _isFilterPopupVisible; }
            set
            {
                if (value)
                    IsFindPopupVisible = false;
                else
                {
                    ResetFilter();
                }
                OnPropertyChanging(() => IsFilterPopupVisible);
                _isFilterPopupVisible = value;
                OnPropertyChanged(() => IsFilterPopupVisible);
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

        public UserViewModel EditingItem
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

        public Visual PrintArea { get; set; }

        protected virtual IEnumerable<UserViewModel> GetUsers(UserListView view)
        {
            return view.Users.Select((u, i) => new UserViewModel
            {
                Id = u.Id.Id,
                Login = u.Login,
                Name = u.Name,
                RoleType = u.RoleType,
                HasChanges = false,
                IsValid = true
            });
        }

        protected void DoSearch(string searchText)
        {
            if (!CollectionView.IsAddingNew && !CollectionView.IsEditingItem)
            {
                CollectionView.Filter = (object t) =>
                {
                    var product = t as ProductViewModel;
                    if (string.IsNullOrEmpty(searchText))
                        return true;
                    return product.ItemNo.IndexOf(searchText) > -1;
                };
            }
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
                    if (!IsLoading)
                        HasChanges = true;
                    break;

                default:
                    break;
            }

            if (e.OldItems != null)
            {
                foreach (UserViewModel item in e.OldItems)
                {
                    Unbind(item, t => t.HasChanges, ProductViewModel_HasChangesPropertyChanged);
                    _session.Command(new DeleteUser(new UserId(item.Id)));
                }
            }
            if (e.NewItems != null)
            {
                foreach (UserViewModel item in e.NewItems)
                {
                    Bind(item, t => t.HasChanges, ProductViewModel_HasChangesPropertyChanged);
                }
            }
        }

        private void ProductViewModel_HasChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (UserViewModel)sender;
            if (item.HasChanges && !_changedProducts.Contains(item))
            {
                _changedProducts.Add(item);
            }
            if (item.HasChanges)
                this.HasChanges = true;
        }

        private void HasChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var end = "*";
            if (HasChanges)
            {
                //todo: CanClose = false;
            }
            if (HasChanges && !DisplayName.EndsWith(end))
            {
                DisplayName = DisplayName + end;
            }
            else if (!HasChanges && DisplayName.EndsWith(end))
            {
                DisplayName = DisplayName.Substring(0, DisplayName.Length - 1);
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
            //        //var documentPaginator = new DataGridDocumentPaginator((DataGrid)PrintArea, string.Empty, new System.Windows.Size(940, 1070), new System.Windows.Thickness());
            //        exporter.Export(PrintArea, saveFileDialog.FileName);
            //        SendMessage(new MessageEnvelope(T("PrintCompleted")));
            //    });
            //}
        }

        public void SaveChanges()
        {
            if (CanSave)
            {
                var saveTask = Task.Factory.StartNew(() =>
                {
                    HasChanges = false;
                    CanClose = true;
                    foreach (var item in Source as IEnumerable<UserViewModel>)
                    {
                        item.HasChanges = false;
                    }
                    MergeChanges();
                    _session.Commit();
                    SendMessage(new MessageEnvelope(T("Saved")));
                });
            }
        }

        protected void MergeChanges()
        {
            foreach (var item in _changedProducts)
            {
                var product = _userListView.Users.Find(t => t.Id == new UserId(item.Id));
                if (product == null)
                {
                    //var userId = new UserId(_session.GetId());
                    //_session.Command(new CreateUser(userId, item.Name, item.Login, item.NewPassword, item.RoleType));
                    //item.Id = userId.Id;
                }
                else
                {
                    if (product.Name != item.Name)
                    {
                        _session.Command(new RenameUser(product.Id, item.Name));
                    }
                    if (product.RoleType != item.RoleType)
                    {
                        _session.Command(new AssignRoleToUser(product.Id, item.RoleType));
                    }
                }
            }
            _changedProducts.Clear();
        }

        public bool CanSave
        {
            get
            {
                if (CollectionView == null)
                    return false;
                return HasChanges && IsValid && CollectionView.OfType<UserViewModel>().All(t => t.IsValid);
            }
        }
    }
}
