using System.Windows;
using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using BookKeeping.UI.Localization;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;

namespace BookKeeping.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;

            ResourceLocalizer.Initialize(BookKeeping.App.Properties.Resources.ResourceManager);

            InitSuperUser();

            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;

            viewModel.RequestClose += (sender, args) =>
            {
                window.Close();
            };
            window.Show();
        }

        protected void InitSuperUser()
        {
            var userIndex = Context.Current.Query<UserIndexLookup>();
            if (userIndex.HasValue)
            {
                if (!userIndex.Value.Logins.ContainsKey("admin"))
                {
                    Context.Current.Command(new CreateUser(new UserId(1), "Адміністратор", "admin", "qwerty", "admin"));
                }
            }
            else
            {
                Context.Current.Command(new CreateUser(new UserId(1), "Адміністратор", "admin", "qwerty", "admin"));
            }
        }
    }
}