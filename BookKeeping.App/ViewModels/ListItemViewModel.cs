using BookKeeping.UI.ViewModels;
using System.Linq;

namespace BookKeeping.App.ViewModels
{
    public class ListItemViewModel : ViewModelBase
    {
        public ListItemViewModel()
        {
            this.PropertyChanged += OnItemPropertyChanged;
            Bind(() => Id, (s, e) => IsNew = Id == 0);
        }

        protected virtual void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var fields = new[] { GetPropertyName(() => HasChanges), GetPropertyName(() => IsHighlight),
                GetPropertyName(() => IsValid), GetPropertyName(() => IsEdit) };
            if (fields.Contains(e.PropertyName))
                return;
            HasChanges = true;
        }

        private long _id;

        public virtual long Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        private bool _isNew;

        public virtual bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                OnPropertyChanged(() => IsNew);
            }
        }

        private bool _isHighlight;

        public virtual bool IsHighlight
        {
            get { return _isHighlight; }
            set
            {
                _isHighlight = value;
                OnPropertyChanged(() => IsHighlight);
            }
        }

        private bool _isEdit = false;

        public virtual bool IsEdit
        {
            get { return _isEdit; }
            set
            {
                _isEdit = value;
                OnPropertyChanged(() => IsEdit);
            }
        }

        private bool _hasChanges;

        public virtual bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                OnPropertyChanged(() => HasChanges);
            }
        }
    }
}
