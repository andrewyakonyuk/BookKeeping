using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BookKeeping.UI.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase, IObservable<MessageEnvelope>
    {
        private string _displayName = string.Empty;
        private readonly List<IObserver<MessageEnvelope>> _observers = new List<IObserver<MessageEnvelope>>();

        public WorkspaceViewModel()
        {
            CloseCommand = new DelegateCommand(o => OnRequestClose(new RoutedEventArgs()));
            CanClose = true;
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (string.Equals(_displayName, value)) return;
                OnPropertyChanging(() => DisplayName);
                _displayName = value;
                OnPropertyChanged(() => DisplayName);
            }
        }

        private bool _canClose = true;

        public bool CanClose
        {
            get { return _canClose; }
            set
            {
                _canClose = value;
                OnPropertyChanged(() => CanClose);
            }
        }

        public ICommand CloseCommand { get; set; }

        public event RoutedEventHandler RequestClose;

        protected void OnRequestClose(RoutedEventArgs eventArgs)
        {
            if (RequestClose != null)
                RequestClose(this, eventArgs);
        }

        public virtual IDisposable Subscribe(IObserver<MessageEnvelope> observer)
        {
            _observers.Add(observer);
            return this as IDisposable;
        }

        public virtual void SendMessage(MessageEnvelope message)
        {
            foreach (var item in _observers)
            {
                item.OnNext(message);
            }
        }

        public virtual void SendError(Exception ex)
        {
            foreach (var item in _observers)
            {
                item.OnError(ex);
            }
        }

        public virtual void Dispose()
        {
            _observers.Clear();
        }
    }
}