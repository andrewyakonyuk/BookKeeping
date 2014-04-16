using System;
using System.ComponentModel;

namespace BookKeeping.App.ViewModels
{
    public class ViewModelBase : NotificationObject, IDataErrorInfo
    {
        public string Error { get; set; }

        public virtual string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

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