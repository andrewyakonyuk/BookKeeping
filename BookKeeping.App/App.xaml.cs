using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using BookKeeping.UI.Localization;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

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

            var culture = System.Configuration.ConfigurationManager.AppSettings["culture"];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);

            ResourceLocalizer.Initialize(BookKeeping.App.Properties.Resources.ResourceManager);
            //todo:
            ServiceLocator.SetLocatorProvider(() => new AutofacContrib.CommonServiceLocator.AutofacServiceLocator(null));

            //  GenerateProducts();

            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;

            viewModel.RequestClose += (sender, args) =>
            {
                window.Close();
            };
            Current.MainWindow = window;
            window.Show();
        }
    }
}