using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using System.Windows;

namespace BookKeeping.App
{
    //TODO: domain entities: 
    //  1) store,
    //  2) warehouse,
    //  3) product,
    //  4) order,
    //  5) vat group,
    //  6) sku,
    //  7) incoming
    //  8) expense
    //TODO: windows: 
    //  1) stores list, create new store, edit store
    //  2) warehouses list, create new warehouse, edit warehouse
    //  3) 
    //TODO: commands:
    //  1) store: 
    //      a. create store (id, store id, title, warehouses, vat group);
    //      b. close store (id, store id)
    //      c. rename (id, store id, new title)
    //      d. change warehouses(id, store id, warehouses)
    //      e. change vat group (id, store id, vat group)
    //  2) warehouses:
    //      a. create warehouse (id, warehouse id, title)
    //      b. close warehouse (id, warehouse id)
    //      c. rename warehouse (id, warehouse id, new title)
    //      d. purchase (id, product id, quantity, purchase price, markup)
    //      e. order (id, product id
    //TODO: extract domain
    //TODO: implement caching and loging
    //TODO: create reports (price-list, invoice)
    //TODO: roles-based user interface
    //TODO: UI - confirm close edit panel, if product data was changed

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