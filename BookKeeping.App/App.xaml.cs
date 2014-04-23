using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using BookKeeping.Domain;
using BookKeeping.Domain.ProductAggregate;
using BookKeeping.Projections;
using System;
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

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture; //new System.Globalization.CultureInfo("uk-Ua");

            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;

            viewModel.RequestClose += (sender, args) =>
            {
                window.Close();
            };
            window.Show();
        }
    }
}