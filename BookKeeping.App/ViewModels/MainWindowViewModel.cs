using BookKeeping.Auth;
using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure;
using BookKeeping.Projections;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using ICommand = System.Windows.Input.ICommand;
using System.Linq;
using System.Threading.Tasks;
using BookKeeping.Domain.Projections.UserIndex;
using System.Collections.Generic;

namespace BookKeeping.App.ViewModels
{
    public class MainWindowViewModel : MainWindowViewModelBase
    {
        private bool _quitConfirmationEnabled;
        private IContextUserProvider _contextUserProvider;
        private readonly IRepository<User,UserId> _repostory;
        private UserId _previousUserId;
        private bool _isWorkspacesVisible;
        private ISession _session = Context.Current.GetSession();

        public MainWindowViewModel()
        {
            this.QuitConfirmationEnabled = true;

            var userRepository = (IUserRepository)((Session)_session).GetRepo<User, UserId>();
            _repostory = userRepository;

            var authService = new AuthenticationService(userRepository);
            SignIn = new SignInViewModel(authService);
            SignIn.AuthenticationSuccessful += AuthorizationSuccessful;

            Profile = new ProfileViewModel(authService);
            Bind(Profile, t => t.IsAuthorization, IsAuthorization_PropertyChanged);
            Bind(this, t => t.IsLoading, (sender, e) => { SignIn.CanSignIn = !IsLoading; });

            MainMenu.Clear();
            BuildMainMenu();

            var cts = new CancellationTokenSource();
            var startupTasks = Task.Factory.StartNew(() =>
            {
                ExecuteStartupTasks(cts.Token);
                IsLoading = false;
                cts.Dispose();
            });
        }

        void AuthorizationSuccessful(object sender, EventArgs e)
        {
            if (_contextUserProvider == null)
                _contextUserProvider = new ContextUserProvider((UserRepository)_repostory);

            Profile.Username = _contextUserProvider.ContextUser().Name;
            Profile.IsAuthorization = true;
            _previousUserId = _contextUserProvider.ContextUser().Id;
        }

        void IsAuthorization_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Profile.IsAuthorization)
            {
                Profile.ChangePassword = new ChangePasswordViewModel(_session);
                if (_contextUserProvider.ContextUser() != null
                    && _previousUserId != _contextUserProvider.ContextUser().Id)
                {
                    Workspaces.Clear();
                }
            }
        }

        public SignInViewModel SignIn { get; private set; }

        public ProfileViewModel Profile { get; private set; }

        #region Commands

        public ICommand CloseLoginCmd { get; private set; }

        public ICommand CloseTabItem { get; set; }

        public ICommand Exit { get; set; }

        public ICommand ListOfProductsCmd { get; private set; }

        public ICommand ListOfCustomersCmd { get; private set; }

        public ICommand VendorListCmd { get; private set; }

        public ICommand SaleOfGoodsCmd { get; private set; }

        public ICommand PurchaseOfGoodsCmd { get; private set; }

        public ICommand SettingsCmd { get; private set; }

        public ICommand EventHistoryCmd { get; private set; }

        public ICommand UserListCmd { get; private set; }

        public ICommand RemainsOfGoodsReportCmd { get; private set; }

        public ICommand HistoryOfGoodsReportCmd { get; private set; }

        public ICommand SalesForPeriodReportCmd { get; private set; }

        public ICommand ReportBySellersReportCmd { get; private set; }

        public ICommand SaveCmd { get; private set; }

        public ICommand PrintCmd { get; private set; }

        #endregion

        public bool IsWorkspacesVisible
        {
            get { return _isWorkspacesVisible; }
            set
            {
                _isWorkspacesVisible = value;
                OnPropertyChanged(() => IsWorkspacesVisible);
            }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

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

        protected override void BuildMainMenu()
        {
            base.BuildMainMenu();

            //General
            PrintCmd = new DelegateCommand(_ => ((IPrintable)CurrentWorkspace).Print(), _ => CurrentWorkspace is IPrintable);
            Exit = ApplicationCommands.Close;
            SaveCmd = new DelegateCommand(_ => ((ISaveable)CurrentWorkspace).SaveChanges(), _ => CurrentWorkspace is ISaveable && ((ISaveable)CurrentWorkspace).CanSave);

            CloseLoginCmd = new DelegateCommand(t => { }, t => Profile.IsAuthorization);

            ListOfProductsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<ProductListViewModel>()));
            SaleOfGoodsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<OrderViewModel>()));
            ListOfCustomersCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<CustomerListViewModel>()));
            VendorListCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<VendorListViewModel>()));

            //Services
            EventHistoryCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<EventHistoryViewModel>()), _ => Current.Identity.RoleType == "admin");
            UserListCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<UserListViewModel>()), _ => Current.Identity.RoleType == "admin");

            // Reports
            RemainsOfGoodsReportCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<RemainsOfGoodsViewModel>()));
            SalesForPeriodReportCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<SalesForPeriodViewModel>()));
            ReportBySellersReportCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<ReportBySellersViewModel>()));
        }

        protected override void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnWorkspacesChanged(sender, e);
            IsWorkspacesVisible = Workspaces.Count > 0;
        }

        public void ExecuteStartupTasks(CancellationToken token)
        {
            StartupProjectionRebuilder.Rebuild(
                token,
                Context.Current.Projections,
                Context.Current.EventStore,
                projections => DomainBoundedContext.Projections(projections)
                .Concat(ClientBoundedContext.Projections(projections)),
                () => { IsLoading = true; return true; });

            InitUsers();

            //GenerateProducts();
        }

        protected void InitUsers()
        {
            using (var session = Context.Current.GetSession())
            {
                var userIndex = session.Query<UserIndexLookup>();
                var createAdminCmd = new CreateUser(new UserId(session.GetId()), "Адміністратор", "admin", "qwerty", "admin");
                var createSellerCmd = new CreateUser(new UserId(session.GetId()), "Продавець 1", "seller", "qwerty", "seller");
                var createAnotherSellerCmd = new CreateUser(new UserId(session.GetId()), "Продавець 2", "anotherseller", "qwerty", "seller");
                if (userIndex.HasValue)
                {
                    if (!userIndex.Value.Logins.ContainsKey("admin"))
                    {
                        session.Command(createAdminCmd);
                    }
                    if (!userIndex.Value.Logins.ContainsKey("seller"))
                    {
                        session.Command(createSellerCmd);
                    }
                    if (!userIndex.Value.Logins.ContainsKey("anotherseller"))
                    {
                        session.Command(createAnotherSellerCmd);
                    }
                }
                else
                {
                    session.Command(createAdminCmd);
                    session.Command(createSellerCmd);
                    session.Command(createAnotherSellerCmd);
                }
                session.Commit();
            }
        }

        protected void GenerateProducts()
        {
            var random = new Random();
            using (var session = Context.Current.GetSession())
            {
                var userRepo = ((Session)session).GetRepo<User, UserId>();
                var identities = new List<IUserIdentity>();

                foreach (var user in userRepo.All())
                {
                    identities.Add(new UserIdentity(new AccountEntry(user), user.Name));
                }

                for (int i = 0; i < 500; i++)
                {
                    BookKeeping.Infrastructure.Current.IdentityIs(identities[random.Next(0, identities.Count - 1)]);
                    session.Command(new CreateProduct(new ProductId(session.GetId()),
                        new string("qwertyuiopasdfghjklzxcvbnm".Substring(random.Next(0, 12)).OrderBy(t => Guid.NewGuid()).ToArray()),
                        "item no. " + (i + 1),
                        new CurrencyAmount(random.Next(10, 100), Currency.Eur),
                        random.Next(1, 1000),
                        "m2",
                        new VatRate(new decimal(random.NextDouble())),
                        new Barcode("12342323", BarcodeType.EAN13)
                        ));
                }
                session.Commit();
            }
            BookKeeping.Infrastructure.Current.Reset();
        }
    }
}