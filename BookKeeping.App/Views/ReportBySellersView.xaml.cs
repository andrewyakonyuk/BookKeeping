using BookKeeping.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookKeeping.App.Views
{
    /// <summary>
    /// Interaction logic for ReportBySellersView.xaml
    /// </summary>
    public partial class ReportBySellersView : UserControl
    {
        public ReportBySellersView()
        {
            InitializeComponent();
            this.DataContextChanged += ChartsView_DataContextChanged;
        }

        void ChartsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as ReportBySellersViewModel;
            viewModel.PrintArea = this.Charts;
        }
    }
}
