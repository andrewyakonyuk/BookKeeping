using System.Windows;
using BookKeeping.App.ViewModels;
using BookKeeping.App.Views;
using BookKeeping.UI.Localization;
using BookKeeping.Domain.Contracts;
using BookKeeping.Domain.Projections.UserIndex;
using System;
using System.Linq;
using BookKeeping.Domain.Aggregates;
using System.Collections.Generic;
using BookKeeping.Auth;

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

            InitUsers();

            //var random = new Random(100);
            //var userRepo = Context.Current.GetRepo<User, UserId>();
            //var identities = new List<IUserIdentity>();

            //foreach (var user in userRepo.All())
            //{
            //    identities.Add(new UserIdentity(new AccountEntry(user), user.Name));
            //}

            //for (int i = 500; i < 1000; i++)
            //{
            //    BookKeeping.Infrastructure.Current.IdentityIs(identities[random.Next(0, identities.Count - 1)]);
            //    Context.Current.Command(new CreateProduct(new ProductId(i),
            //        new string("qwertyuiopasdfghjklzxcvbnm".Substring(random.Next(0, 12)).OrderBy(t => Guid.NewGuid()).ToArray()),
            //        "item no. " + (i + 1),
            //        new CurrencyAmount(random.Next(10, 100), Currency.Eur),
            //        random.Next(1, 1000),
            //        "m2",
            //        new VatRate(new decimal(random.NextDouble())),
            //        new Barcode("12342323", BarcodeType.EAN13)
            //        ));
            //}
            BookKeeping.Infrastructure.Current.Reset();

            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;

            viewModel.RequestClose += (sender, args) =>
            {
                window.Close();
            };
            window.Show();
        }

        protected void InitUsers()
        {
            var userIndex = Context.Current.Query<UserIndexLookup>();
            var createAdminCmd = new CreateUser(new UserId(1), "Адміністратор", "admin", "qwerty", "admin");
            var createSellerCmd = new CreateUser(new UserId(2), "Продавець 1", "seller", "qwerty", "seller");
            var createAnotherSellerCmd = new CreateUser(new UserId(3), "Продавець 2", "anotherseller", "qwerty", "seller");
            if (userIndex.HasValue)
            {
                if (!userIndex.Value.Logins.ContainsKey("admin"))
                {
                    Context.Current.Command(createAdminCmd);
                }
                if (!userIndex.Value.Logins.ContainsKey("seller"))
                {
                    Context.Current.Command(createSellerCmd);
                }
                if (!userIndex.Value.Logins.ContainsKey("anotherseller"))
                {
                    Context.Current.Command(createAnotherSellerCmd);
                }
            }
            else
            {
                Context.Current.Command(createAdminCmd);
                Context.Current.Command(createSellerCmd);
                Context.Current.Command(createAnotherSellerCmd);
            }
        }
    }
}