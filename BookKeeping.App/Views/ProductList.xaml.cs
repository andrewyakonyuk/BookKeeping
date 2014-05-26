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
using BookKeeping.App.ViewModels;

namespace BookKeeping.App.Views
{
    /// <summary>
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductList : UserControl
    {
        public ProductList()
        {
            InitializeComponent();

            this.DataContextChanged += ProductList_DataContextChanged;

            this.txtFilterBox.KeyUp += txtFilterBox_KeyUp;
        }

        void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as ProductListViewModel;
            if (viewModel.EditProductCmd.CanExecute(null))
                viewModel.EditProductCmd.Execute(null);
        }

        void txtFilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (ProductListViewModel)DataContext;
            if (e.Key == Key.Enter)
            {
                viewModel.FilterButtonCmd.Execute(new object());
            }
        }

        void ProductList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (ProductListViewModel)DataContext;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, (s, args) =>
            {
                if (DataContext == null)
                    return;
                viewModel.ShowFindPopup = !viewModel.ShowFindPopup;
                args.Handled = true;
            }));

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: should refact
            var datagrid = (DataGrid)sender;
            ((ProductListViewModel)DataContext).SelectedItems = datagrid.SelectedItems;
            if (datagrid.SelectedItems.Count != 1)
                ((ProductListViewModel)DataContext).ShowProductDetail = false;
        }


    }
}
