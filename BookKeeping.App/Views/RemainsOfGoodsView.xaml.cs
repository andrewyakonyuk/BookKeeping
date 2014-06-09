using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
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
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using BookKeeping.App.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace BookKeeping.App.Views
{
    /// <summary>
    /// Interaction logic for RemainsOfGoodsView.xaml
    /// </summary>
    public partial class RemainsOfGoodsView : UserControl
    {
        public RemainsOfGoodsView()
        {
            InitializeComponent();
            this.DataContextChanged += RemainsOfGoodsView_DataContextChanged;
        }

        void RemainsOfGoodsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as RemainsOfGoodsViewModel;
            viewModel.DocumentPaginator = ((IDocumentPaginatorSource)Viewer.Document).DocumentPaginator;
        }
    }
}
