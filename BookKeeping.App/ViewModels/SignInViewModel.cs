using BookKeeping.Auth;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookKeeping.App.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private bool _isSuccessful = true;
        private IAuthenticationService _authService;

        public event EventHandler AuthenticationSuccessful = (sender, e) => { };

        public SignInViewModel(IAuthenticationService authService)
        {
            _authService = authService;

            SignInCmd = new DelegateCommand(passwordBox =>
            {
                OnPropertyChanged(() => Login);     //hack
                IsSuccessful = SignIn(Login, ((PasswordBox)passwordBox).Password);
                if (IsSuccessful)
                {
                    AuthenticationSuccessful(this, new EventArgs());
                    Login = string.Empty;
                    ((PasswordBox)passwordBox).Password = string.Empty;
                }
            });
        }

        public string Login
        {
            get { return _login; }
            set
            {
                OnPropertyChanging(() => Login);
                _login = value;
                OnPropertyChanged(() => Login);
            }
        }

        public bool IsSuccessful
        {
            get { return _isSuccessful; }
            set
            {
                OnPropertyChanging(() => IsSuccessful);
                _isSuccessful = value;
                OnPropertyChanged(() => IsSuccessful);
            }
        }

        protected override string GetErrorMessage(string columnName)
        {
            if (IsSuccessful == true)
                return null;
            switch (columnName)
            {
                case "Login":
                    return T("SignInFailed");

                case "Password":
                    return T("SignInFailed");

                default:
                    return null;
            }
        }

        public ICommand SignInCmd { get; private set; }

        public bool SignIn(string login, string password)
        {
            return _authService.SignIn(login, password);
        }
    }
}
