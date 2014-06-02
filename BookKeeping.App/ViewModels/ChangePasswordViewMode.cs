using BookKeeping.Auth;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Repositories;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace BookKeeping.App.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private User _user;
        private bool _isVisible = false;
        private readonly IRepository<User,UserId> _repository;

        public ChangePasswordViewModel(IRepository<User,UserId> repository)
        {
            _repository = repository;
            IsVisible = false;//TODO:

            ChangePasswordCmd = new DelegateCommand(passwordBox =>
            {

            });
        }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(() => User);
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


        public void ChangePassword(string oldPassword, string newPassword)
        {
            //if (_user != null)
            //{
            //    if (_user.Password.Check(oldPassword))
            //    {
            //        _user.SetPassword(newPassword);
            //    }
            //    _repository.Save(_user);
            //}
        }

        public System.Windows.Input.ICommand ChangePasswordCmd { get; private set; }


    }
}
