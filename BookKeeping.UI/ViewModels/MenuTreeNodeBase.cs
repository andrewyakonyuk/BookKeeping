using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

namespace BookKeeping.UI.ViewModels
{
    public class MenuTreeNodeBase<T> : TreeNode<T>
        where T : TreeNode<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<S>(Expression<Func<S>> action)
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

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null) handler(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanging<S>(Expression<Func<S>> action)
        {
            var propertyName = GetPropertyName(action);
            OnPropertyChanging(propertyName);
        }

        private static string GetPropertyName<S>(Expression<Func<S>> action)
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
                string msg = "Invalid property name: " + propertyName;
                if (ThrowOnInvalidPropertyName)
                    throw new InvalidOperationException(msg);
                else
                    Debug.Fail(msg);
                Debug.Fail(msg);
            }
        }

        protected bool ThrowOnInvalidPropertyName { get; set; }
    }
}