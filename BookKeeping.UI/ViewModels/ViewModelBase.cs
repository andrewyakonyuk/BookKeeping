using System;
using System.ComponentModel;
using BookKeeping.UI.Localization;

namespace BookKeeping.UI.ViewModels
{
    public abstract class ViewModelBase :  NotificationObject, IDataErrorInfo
    {

        public ViewModelBase()
        {
            T = ResourceLocalizer.Instance;
        }

        #region Реализация интерфейса IDataErrorInfo 

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

        #endregion

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

    }
}