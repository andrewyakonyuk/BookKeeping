using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.ViewModels
{
    public class UserViewModel : ListItemViewModel
    {
        public UserViewModel()
        {
            Roles = new List<string> { "admin", "seller" };
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

        public IEnumerable<string> Roles { get; private set; }

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
