using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using BookKeeping.Core.AtomicStorage;
using BookKeeping.Domain;
using BookKeeping.Projections;
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

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU"); ;

            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel()
            {
                DisplayName = "Main window"
            };
            window.DataContext = viewModel;

            viewModel.RequestClose += (sender, args) =>
            {
                window.Close();
            };
            window.Show();
        }
    }
}