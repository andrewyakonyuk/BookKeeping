using System;
using System.ComponentModel;
using BookKeeping.UI.Localization;
using System.Linq.Expressions;

namespace BookKeeping.UI.ViewModels
{
    public abstract class ViewModelBase :  NotificationObject, IDataErrorInfo
    {
        private bool _isValid = true;

        public ViewModelBase()
        {
            T = ResourceLocalizer.Instance;
        }
        public string Error { get; set; }

        public virtual string this[string columnName]
        {
            get { return GetErrorMessage(columnName); }
        }

        protected virtual string GetErrorMessage(string columnName)
        {
            return null;
        }

        public virtual Localizer T { get; set; }

        private object _source;

        public object Source
        {
            get { return _source; }
            set
            {
                if (_source == value) return;
                OnPropertyChanging(() => Source);
                _source = value;
                OnPropertyChanged(() => Source);
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                OnPropertyChanged(() => IsValid);
            }
        }

    }
}