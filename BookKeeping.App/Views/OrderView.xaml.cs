using BookKeeping.App.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace BookKeeping.App.Views
{
    /// <summary>
    /// Interaction logic for BasketView.xaml
    /// </summary>
    public partial class OrderView : UserControl
    {
        public OrderView()
        {
            InitializeComponent();
            this.DataContextChanged += OrderView_DataContextChanged;
        }

        void OrderView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = this.DataContext as OrderViewModel;
            if (viewModel != null)
            {
                viewModel.CheckoutCompleted += viewModel_CheckoutCompleted;
            }
        }

        private async void viewModel_CheckoutCompleted(object sender, EventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["SimpleDialogTest"];

            await ((MetroWindow)App.Current.MainWindow).ShowMetroDialogAsync(dialog);

            await TaskEx.Delay(2000);

            await ((MetroWindow)App.Current.MainWindow).HideMetroDialogAsync(dialog);
        }
    }
}
