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

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, (sender, e) =>
            {
                if (DataContext == null)
                    return;
                ((ProductListViewModel)DataContext).ShowFindPopup = !((ProductListViewModel)DataContext).ShowFindPopup;
                e.Handled = true;
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
