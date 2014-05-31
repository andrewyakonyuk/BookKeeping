using System;
using System.ComponentModel;
using BookKeeping.UI.Localization;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace BookKeeping.UI.ViewModels
{
    public abstract class ViewModelBase :  NotificationObject, IDataErrorInfo
    {
        private bool _isValid = true;
        private readonly Dictionary<Tuple<INotifyPropertyChanged, string>, IList<EventHandler<PropertyChangedEventArgs>>> EventHandlers;

        public ViewModelBase()
        {
            T = ResourceLocalizer.Instance;
            EventHandlers = new Dictionary<Tuple<INotifyPropertyChanged, string>, IList<EventHandler<PropertyChangedEventArgs>>>();
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

        public void Bind(INotifyPropertyChanged element, string property, EventHandler<PropertyChangedEventArgs> callback)
        {
            var pair = new Tuple<INotifyPropertyChanged, string>(element, property);
            if (EventHandlers.ContainsKey(pair))
            {
                EventHandlers[pair].Add(callback);
            }
            else
            {
                var eventsList = new List<EventHandler<PropertyChangedEventArgs>> { callback };
                EventHandlers.Add(pair, eventsList);
                element.PropertyChanged += element_PropertyChanged;
            }
        }

        public void Bind<T>(Expression<Func<T>> property, EventHandler<PropertyChangedEventArgs> callback)
        {
            Bind(this, GetPropertyName(property), callback);
        }

        public void Bind<N, P>(N element, Expression<Func<N, P>> action, EventHandler<PropertyChangedEventArgs> callback)
            where N : INotifyPropertyChanged
        {
            var propertyName = GetPropertyName(action);
            Bind(element, propertyName, callback);
        }

        public void Unbind(INotifyPropertyChanged element, string property,
                                  EventHandler<PropertyChangedEventArgs> callback)
        {
            var pair = new Tuple<INotifyPropertyChanged, string>(element, property);
            if (EventHandlers.ContainsKey(pair))
            {
                EventHandlers[pair].Remove(callback);
            }
        }

        public void Unbind<N, P>(N element, Expression<Func<N, P>> action, EventHandler<PropertyChangedEventArgs> callback)
            where N : INotifyPropertyChanged
        {
            var propertyName = GetPropertyName(action);
            Unbind(element, propertyName, callback);
        }

        private void element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pair = new Tuple<INotifyPropertyChanged, string>(sender as INotifyPropertyChanged, e.PropertyName);
            if (EventHandlers.ContainsKey(pair))
            {
                var handlers = EventHandlers[pair];
                foreach (EventHandler<PropertyChangedEventArgs> eventHandler in EventHandlers[pair])
                {
                    eventHandler(sender, e);
                }
            }
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        private static string GetPropertyName<T, S>(Expression<Func<T, S>> action)
        {
            var expression = (MemberExpression)action.Body;
            return expression.Member.Name;
        }

    }
}