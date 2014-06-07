using BookKeeping.App.Helpers;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICommand = System.Windows.Input.ICommand;

namespace BookKeeping.App.ViewModels
{
    public abstract class ListViewModel<TItem> : WorkspaceViewModel, ISaveable
        where TItem : ListItemViewModel
    {
        private object _selectedItem;
        private IList _selectedItems;
        private bool _hasChanges = false;
        private TItem _editingItem;
        private TItem _previousEditingItem;
        private readonly ExpressionHelper _expressionHelper = new ExpressionHelper();
        private bool _isLoading;
        private readonly List<TItem> _changedItems = new List<TItem>();

        protected ListViewModel()
        {
            CanEdit = () => SelectedItems.Count == 1;
            CanSave = () => CollectionView == null ? false : HasChanges && IsValid && CollectionView.OfType<ViewModelBase>().All(t => t.IsValid);
            CanSearch = () => true;
            CanFilter = () => true;

            SearchPopup = new PopupViewModel();
            FilterPopup = new PopupViewModel();

            SearchPopup.ActionCmd = new DelegateCommand(_ => DoSearch(SearchPopup.Text), _ => CanSearch());
            SearchPopup.CloseCmd = new DelegateCommand(_ => { SearchPopup.IsVisible = false; CollectionView.Filter = t => true; });
            SearchPopup.OpenCmd = new DelegateCommand(_ =>  SearchPopup.IsVisible = true, _ => CanSearch());
            SearchPopup.Placeholder = T("DoSearch");

            FilterPopup.ActionCmd = new DelegateCommand(_ => DoFilter(FilterPopup.Text), _ => CanFilter());
            FilterPopup.CloseCmd = new DelegateCommand(_ => { FilterPopup.IsVisible = false; ResetFilter(); });
            FilterPopup.OpenCmd = new DelegateCommand(_ => FilterPopup.IsVisible = true, _ => CanFilter());
            FilterPopup.Placeholder = T("DoFilter");

            EditItemCmd = new DelegateCommand(item => { EditingItem = item == EditingItem ? null : item as TItem; }, _ => CanEdit());
            SaveCmd = new DelegateCommand(_ => SaveChanges(), _ => CanSave());

            IsLoading = true;

            Bind(() => HasChanges, HasChangesPropertyChanged);

            var tempSource = new ObservableCollection<TItem>();

            tempSource.CollectionChanged += tempSource_CollectionChanged;
            Task loadItemsTask = Task.Factory.StartNew(() =>
            {
                foreach (var item in LoadItems())
                {
                    tempSource.Add(item);
                }
                Source = tempSource;
                HasChanges = false;
                IsLoading = false;
            });
        }

        public virtual bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

        public virtual bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                OnPropertyChanged(() => HasChanges);
            }
        }

        public PopupViewModel SearchPopup { get; private set; }

        public PopupViewModel FilterPopup { get; private set; }

        public virtual object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                OnPropertyChanging(() => SelectedItem);
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }

        public virtual IList SelectedItems
        {
            get { return (IList)_selectedItems; }
            set
            {
                OnPropertyChanging(() => SelectedItems);
                _selectedItems = value;
                OnPropertyChanged(() => SelectedItems);
            }
        }

        public virtual TItem EditingItem
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

        public ICommand EditItemCmd { get; protected set; }

        public ICommand SaveCmd { get; protected set; }

        public ListCollectionView CollectionView { get { return (ListCollectionView)CollectionViewSource.GetDefaultView(Source); } }

        public Func<bool> CanSave { get; protected set; }

        public Func<bool> CanEdit { get; protected set; }

        public Func<bool> CanSearch { get; protected set; }

        public Func<bool> CanFilter { get; protected set; }

        protected List<TItem> ChangedItems { get { return _changedItems; } }

        protected abstract IEnumerable<TItem> LoadItems();

        protected void Search(string searchText)
        {
            if (!CollectionView.IsAddingNew && !CollectionView.IsEditingItem)
            {
                CollectionView.Filter = (object t) => true;
            }
        }

        protected virtual void DoSearch(string searchText)
        {
            CollectionView.Filter = (object t) => true;
        }

        protected virtual void DoFilter(string filterExpression)
        {
            Func<TItem, bool> selector = (p) => false;
            try
            {
                var filterSelector = _expressionHelper.GetFilter<TItem>(filterExpression);
                if (filterExpression != null)
                    selector = filterSelector;
            }
            catch (Exception ex)
            {
                //todo: logging
            }
            finally
            {
                foreach (var item in ((IEnumerable<TItem>)Source))
                {
                    if (selector(item))
                        item.IsHighlight = true;
                    else item.IsHighlight = false;
                }
            }
        }

        protected virtual void ResetFilter()
        {
            FilterPopup.Text = string.Empty;
            foreach (var item in ((IEnumerable<TItem>)Source))
            {
                item.IsHighlight = false;
            }
        }

        protected virtual void OnDeletingItem(TItem item)
        {

        }

        protected virtual void OnAddingItem(TItem item)
        {

        }

        protected virtual void HasChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
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

        public virtual void SaveChanges()
        {
            if (CanSave())
            {
                var saveTask = Task.Factory.StartNew(() =>
                {
                    HasChanges = false;
                    CanClose = true;
                    foreach (var item in Source as IEnumerable<TItem>)
                    {
                        item.HasChanges = false;
                    }
                    DoSave();
                    ChangedItems.Clear();
                    SendMessage(new MessageEnvelope(T("Saved")));
                });
            }
        }

        protected abstract void DoSave();

        private void TViewModelItem_HasChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (TItem)sender;
            if (item.HasChanges && !_changedItems.Contains(item))
            {
                _changedItems.Add(item);
            }
            if (item.HasChanges)
                this.HasChanges = true;
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
                foreach (TItem item in e.OldItems)
                {
                    Unbind(item, t => t.HasChanges, TViewModelItem_HasChangesPropertyChanged);
                    OnDeletingItem(item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (TItem item in e.NewItems)
                {
                    Bind(item, t => t.HasChanges, TViewModelItem_HasChangesPropertyChanged);
                    OnAddingItem(item);
                }
            }
        }
    }
}
