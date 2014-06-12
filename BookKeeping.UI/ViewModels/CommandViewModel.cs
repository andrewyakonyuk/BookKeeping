using System;
using System.Windows.Input;

namespace BookKeeping.UI.ViewModels
{
    public class CommandViewModel : ViewModelBase
    {
        private string _title = string.Empty;
        private int _orderNo = 0;
        private bool _visible = true;

        public CommandViewModel(string title, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            Title = title;
            Command = command;
        }

        public ICommand Command { get; private set; }

        public string Title
        {
            get { return _title; }
            set
            {
                OnPropertyChanging(() => Title);
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        public int OrderNo
        {
            get { return _orderNo; }
            set
            {
                OnPropertyChanging(() => OrderNo);
                _orderNo = value;
                OnPropertyChanged(() => OrderNo);
            }
        }

        public string Name { get; set; }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                OnPropertyChanging(() => Visible);
                _visible = value;
                OnPropertyChanged(()=>Visible);
            }
        }
    }
}