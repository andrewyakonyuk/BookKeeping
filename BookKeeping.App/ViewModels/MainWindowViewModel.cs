using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using ICommand = System.Windows.Input.ICommand;
using BookKeeping.UI.ViewModels;
using BookKeeping.UI;
using BookKeeping.Auth;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Aggregates;

namespace BookKeeping.App.ViewModels
{
    public class MainWindowViewModel : MainWindowViewModelBase
    {
        private bool _quitConfirmationEnabled;
        private IContextUserProvider _contextUserProvider;
        private readonly IRepository<User> _repostory;
        private long? _previousUserId;

        public MainWindowViewModel()
        {
            this.DisplayName = BookKeeping.App.Properties.Resources.Application_Name;
            this.QuitConfirmationEnabled = true;

            var userRepository = new UserRepository();
            _repostory = userRepository;

            var authService = new AuthenticationService(userRepository);
            SignIn = new SignInViewModel(authService);
            SignIn.AuthenticationSuccessful += (sender, e) =>
            {
                if (_contextUserProvider == null)
                    _contextUserProvider = new ContextUserProvider(userRepository);

                Profile.Username = _contextUserProvider.ContextUser().Name;
                Profile.IsAuthorization = true;
                _previousUserId = _contextUserProvider.ContextUser().Id;
            };

            Profile = new ProfileViewModel(authService, _contextUserProvider);
            Profile.PropertyChanged += Profile_PropertyChanged;

            CloseLoginCmd = new DelegateCommand(t => { }, t => Profile.IsAuthorization);

            CollectionViewSource.GetDefaultView(Workspaces).MoveCurrentToFirst();
            Exit = ApplicationCommands.Close;

            SaveCmd = new DelegateCommand(_ =>
            {
                if (CurrentWorkspace is ProductListViewModel)
                {
                    ((ProductListViewModel)CurrentWorkspace).SaveCmd.Execute(_);
                }
            },
            _ =>
            {
                if (CurrentWorkspace is ProductListViewModel)
                {
                    return ((ProductListViewModel)CurrentWorkspace).SaveCmd.CanExecute(_);
                }
                return false;
            });

            MainMenu.Clear();
            BuildMainMenu();
        }

        void Profile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsAuthorization":
                    if (Profile.IsAuthorization)
                    {
                        Profile.ChangePassword = new ChangePasswordViewModel(_repostory);
                        if (_contextUserProvider.ContextUser() != null
                            && _previousUserId != _contextUserProvider.ContextUser().Id)
                        {
                            Workspaces.Clear();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public SignInViewModel SignIn { get; private set; }

        public ProfileViewModel Profile { get; private set; }

        public ICommand CloseLoginCmd { get; private set; }

        public ICommand CloseTabItem { get; set; }

        public ICommand Exit { get; set; }

        public ICommand ListOfProductsCmd { get; private set; }

        public ICommand ListOfCustomersCmd { get; private set; }

        public ICommand VendorListCmd { get; private set; }

        public ICommand SaleOfGoodsCmd { get; private set; }

        public ICommand PurchaseOfGoodsCmd { get; private set; }

        public ICommand SettingsCmd { get; private set; }

        public ICommand RemainsOfGoodsReportCmd { get; private set; }

        public ICommand HistoryOfGoodsReportCmd { get; private set; }

        public ICommand ChartsCmd { get; private set; }

        public bool QuitConfirmationEnabled
        {
            get { return _quitConfirmationEnabled; }
            set
            {
                if (value.Equals(_quitConfirmationEnabled)) return;
                _quitConfirmationEnabled = value;
                OnPropertyChanged(() => QuitConfirmationEnabled);
            }
        }

        public ICommand SaveCmd { get; private set; }

        protected override void BuildMainMenu()
        {
            base.BuildMainMenu();

            ListOfProductsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<ProductListViewModel>()));
            SaleOfGoodsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<OrderViewModel>()));
            ChartsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<ChartsViewModel>()));
        }
    }
}