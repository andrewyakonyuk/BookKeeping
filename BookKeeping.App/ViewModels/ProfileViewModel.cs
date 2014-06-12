using BookKeeping.Auth;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BookKeeping.App.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IContextUserProvider _userProvider;
        private string _username;
        private bool _isOpen = false;
        private bool _isAuthorization = false;
        private ChangePasswordViewModel _changePassword;

        public ProfileViewModel(IAuthenticationService authService, IContextUserProvider userProvider)
        {
            _authService = authService;
            _userProvider = userProvider;

            SignOutCmd = new DelegateCommand(_ => SignOut());

            OpenProfile = new DelegateCommand(_ => IsOpen = true, _ => IsAuthorization);
        }

        public void SignOut()
        {
            IsOpen = false;
            _authService.SignOut();
            IsAuthorization = false;
            Username = string.Empty;
        }

        public ICommand SignOutCmd { get; private set; }

        public ICommand OpenProfile { get; private set; }

        public string Username
        {
            get { return _username; }
            set
            {
                OnPropertyChanging(() => Username);
                _username = value;
                OnPropertyChanged(() => Username);
            }
        }

        public ChangePasswordViewModel ChangePassword
        {
            get { return _changePassword; }
            set
            {
                _changePassword = value;
                OnPropertyChanged(() => ChangePassword);
            }
        }

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                OnPropertyChanging(() => IsOpen);
                _isOpen = value;
                OnPropertyChanged(() => IsOpen);
            }
        }

        public bool IsAuthorization
        {
            get { return _isAuthorization; }
            set
            {
                OnPropertyChanging(() => IsAuthorization);
                _isAuthorization = value;
                OnPropertyChanged(() => IsAuthorization);
            }
        }
    }
}
