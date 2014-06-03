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
        private bool _isValidationMessageVisible = false;
        private string _validationMessage = string.Empty;

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
                else
                {
                    IsValidationMessageVisible = true;
                    ValidationMessage = T("SignInFailed");
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

        public bool IsValidationMessageVisible
        {
            get { return _isValidationMessageVisible; }
            set
            {
                _isValidationMessageVisible = value;
                OnPropertyChanged(() => IsValidationMessageVisible);
            }
        }

        public string ValidationMessage
        {
            get { return _validationMessage; }
            set
            {
                _validationMessage = value;
                OnPropertyChanged(() => ValidationMessage);
            }
        }

        protected override string GetErrorMessage(string columnName)
        {
            return null;
        }

        public ICommand SignInCmd { get; private set; }

        public bool SignIn(string login, string password)
        {
            return _authService.SignIn(login, password);
        }
    }
}
