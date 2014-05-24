using System.Windows;
using System.Windows.Input;

namespace BookKeeping.UI.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
    {
        private string _displayName = string.Empty;

        public WorkspaceViewModel()
        {
            CloseCommand = new DelegateCommand(o => OnRequestClose(new RoutedEventArgs()));
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (string.Equals(_displayName, value)) return;
                OnPropertyChanging("DisplayName");
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public ICommand CloseCommand { get; set; }

        public event RoutedEventHandler RequestClose;

        protected void OnRequestClose(RoutedEventArgs eventArgs)
        {
            if (RequestClose != null)
                RequestClose(this, eventArgs);
        }
    }
}