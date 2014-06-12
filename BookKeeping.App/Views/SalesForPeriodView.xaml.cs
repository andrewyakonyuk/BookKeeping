using BookKeeping.App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BookKeeping.App.Views
{
    /// <summary>
    /// Interaction logic for SalesForPeriodView.xaml
    /// </summary>
    public partial class SalesForPeriodView : UserControl
    {
        public SalesForPeriodView()
        {
            InitializeComponent();
            this.DataContextChanged += ChartsView_DataContextChanged;
        }

        void ChartsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as SalesForPeriodViewModel;
            viewModel.PrintArea = this.Charts;
        }
    }
}
