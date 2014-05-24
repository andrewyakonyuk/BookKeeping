using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BookKeeping.UI
{
    public abstract class NotificationObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Реализация интерфейса INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Реализация интерфейса INotifyPropertyChanging

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null) handler(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanging<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            OnPropertyChanging(propertyName);
        }

        #endregion

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

        #endregion

    }
}
