using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        public UserViewModel()
        {
            Roles = new List<string> { "admin", "seller" };

            this.PropertyChanged += UserViewModel_PropertyChanged;
            Bind(() => Id, (s, e) => IsNew = Id == 0);
        }

        void UserViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetPropertyName(() => HasChanges)
               || e.PropertyName == GetPropertyName(() => IsHighlight)
               || e.PropertyName == GetPropertyName(() => IsValid)
               || e.PropertyName == GetPropertyName(() => IsEdit))
                return;
            HasChanges = true;
        }

        private long _id;

        public long Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        private string _login;

        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged(() => Login);
            }
        }

        private string _password;

        public string NewPassword
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(() => NewPassword);
            }
        }

        private string _roleType;

        public string RoleType
        {
            get { return _roleType; }
            set
            {
                _roleType = value;
                OnPropertyChanged(() => RoleType);
            }
        }

        private bool _isNew;

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                OnPropertyChanged(() => IsNew);
            }
        }
        

        public IEnumerable<string> Roles { get; private set; }

        private bool _isHighlight;

        public bool IsHighlight
        {
            get { return _isHighlight; }
            set
            {
                _isHighlight = value;
                OnPropertyChanged(() => IsHighlight);
            }
        }

        private bool _isEdit = false;

        public bool IsEdit
        {
            get { return _isEdit; }
            set
            {
                _isEdit = value;
                OnPropertyChanged(() => IsEdit);
            }
        }

        private bool _hasChanges;

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                OnPropertyChanged(() => HasChanges);
            }
        }

        protected override string GetErrorMessage(string columnName)
        {
            string error = null;
            switch (columnName)
            {
                case "Name":
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        error = T("ShouldBeNotEmpty");
                    };
                    break;
                //case "NewPassword":
                //    if (string.IsNullOrWhiteSpace(NewPassword) && IsNew)
                //    {
                //        error = T("ShouldBeNotEmpty");
                //    };
                //    break;
                default:
                    break;
            }
            return error;
        }
    }
}
