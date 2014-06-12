using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BookKeeping.App.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        private string _text = string.Empty;

        public virtual string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(() => Text);
            }
        }

        private string _placeholder;

        public string Placeholder
        {
            get { return _placeholder; }
            set
            {
                _placeholder = value;
                OnPropertyChanged(() => Placeholder);
            }
        }

        private bool _isVisible;

        public virtual bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged(() => IsVisible);
            }
        }

        public ICommand ActionCmd { get; set; }

        public ICommand CloseCmd { get; set; }

        public ICommand OpenCmd { get; set; }
    }
}
