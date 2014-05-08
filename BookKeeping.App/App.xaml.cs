using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using BookKeeping.Domain.Contracts;
using System;
using System.Windows;
using System.Linq;

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

            using (var context = Context.Current)
            {
                var random = new Random(Guid.NewGuid().ToByteArray()[0]);

                for (int i = 0; i < 100; i++)
                {
                    context.Send(new CreateSku
                    {
                        Id = new SkuId(Guid.NewGuid().ToString()),
                        ItemNo = "item no. " + Guid.NewGuid().ToString().Substring(0, 6),
                        Stock = random.Next(0, 1000),
                        Title = string.Concat("qwertyuiopasdfghjklzxcvbnm".OrderBy(t => Guid.NewGuid())).Substring(0, random.Next(3, 24)),
                        Price = new CurrencyAmount(random.Next(0, 1000), Currency.Eur),
                        UnitOfMeasure = "m2",
                        VatRate = new VatRate(21),
                        Warehouse = new WarehouseId(Guid.Parse("9B3D7441-0615-4F6A-8EE8-2EC5DEDE48A6"))
                    });
                }
                context.Commit();
            }

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