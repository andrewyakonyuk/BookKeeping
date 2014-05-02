using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace BookKeeping
{
    [Serializable]
    public abstract class NotificationObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Реализация интерфейса INotifyPropertyChanged

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> action)
        {
            Contract.Requires(action != null);

            var propertyName = GetPropertyName(action);
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Реализация интерфейса INotifyPropertyChanged

        #region Реализация интерфейса INotifyPropertyChanging

        public virtual event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null) handler(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanging<T>(Expression<Func<T>> action)
        {
            Contract.Requires(action != null);

            var propertyName = GetPropertyName(action);
            OnPropertyChanging(propertyName);
        }

        #endregion Реализация интерфейса INotifyPropertyChanging

        #region Вспомогательные члены класса

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Неверное имя свойства: " + propertyName;
                if (ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
                Debug.Fail(msg);
            }
        }

        protected bool ThrowOnInvalidPropertyName { get; set; }

        #endregion Вспомогательные члены класса
    }
}