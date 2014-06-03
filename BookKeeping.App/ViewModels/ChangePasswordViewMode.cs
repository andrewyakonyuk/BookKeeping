using BookKeeping.Domain;
using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.App.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly Session _session;
        private bool _isValidationMessageVisible = false;
        private bool _isVisible = false;
        private string _newPassword;
        private string _oldPassword;
        private string _validationMessage;

        public ChangePasswordViewModel(Session session)
        {
            IsVisible = true;
            _session = session;

            ChangePasswordCmd = new DelegateCommand(_ =>
            {
                ChangePassword(OldPassword, NewPassword);
            }, _ => !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(OldPassword));
        }

        public System.Windows.Input.ICommand ChangePasswordCmd { get; private set; }

        public bool IsValidationMessageVisible
        {
            get { return _isValidationMessageVisible; }
            set
            {
                _isValidationMessageVisible = value;
                OnPropertyChanged(() => IsValidationMessageVisible);
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged(() => IsVisible);
            }
        }

        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged(() => NewPassword);
            }
        }

        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                _oldPassword = value;
                OnPropertyChanged(() => OldPassword);
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

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (Current.Identity != null)
            {
                try
                {
                    _session.Command(new ChangeUserPassword(Current.Identity.Id, oldPassword, newPassword));
                    _session.Commit();
                    ValidationMessage = T("ChangePasswordSuccessful");
                    IsValidationMessageVisible = true;
                }
                catch (InvalidDomainOperationException)
                {
                    ValidationMessage = T("ChangePasswordFailed");
                    IsValidationMessageVisible = true;
                }
            }
        }
    }
}