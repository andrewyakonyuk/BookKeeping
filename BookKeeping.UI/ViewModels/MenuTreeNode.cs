using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BookKeeping.UI.ViewModels
{
    public class MenuTreeNode : MenuTreeNodeBase<MenuTreeNode>
    {
        private string _title = string.Empty;
        private int _orderNo = 0;
        private bool _visible = true;
        private BitmapImage _menuIcon;

        public MenuTreeNode(string title, ICommand command)
        {
            Title = title;
            Command = command;
        }


        public BitmapImage MenuIcon
        {
            get { return _menuIcon; }
            set
            {
                if (_menuIcon == value)
                    return;
                OnPropertyChanging(() => MenuIcon);
                _menuIcon = value;
                OnPropertyChanged(() => MenuIcon);
            }
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
                OnPropertyChanged(() => Visible);
            }
        }
    }
}