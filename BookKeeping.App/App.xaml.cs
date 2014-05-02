using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
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

            var context = Context.Current;

            //var watcher = new System.Diagnostics.Stopwatch();
            //watcher.Start();
            //var random = new Random(Guid.NewGuid().ToByteArray()[0]);
            //for (int i = 0; i < 1000; i++)
            //{
            //    context.CommandBus.Send(new CreateProduct
            //    {
            //        Id = new ProductId(Guid.NewGuid()),
            //        ItemNo = "item no. " + Guid.NewGuid().ToString().Substring(0, 6),
            //        Stock = random.Next(0, 1000),
            //        Title = string.Concat("qwertyuiopasdfghjklzxcvbnm".OrderBy(t => Guid.NewGuid())),
            //        Price = new CurrencyAmount(random.Next(), Currency.Eur)
            //    });
            //}
            //watcher.Stop();
            //var time = watcher.Elapsed;

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