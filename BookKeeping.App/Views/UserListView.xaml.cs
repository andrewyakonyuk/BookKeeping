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
    /// Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserListView : UserControl
    {
        public UserListView()
        {
            InitializeComponent();
            this.DataContextChanged += ProductList_DataContextChanged;
        }

        public void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as UserListViewModel;
            if (viewModel.EditItemCmd.CanExecute(null))
                viewModel.EditItemCmd.Execute(((Control)sender).DataContext);
        }

        void ProductList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (UserListViewModel)DataContext;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, (s, args) =>
            {
                if (DataContext == null)
                    return;
                viewModel.SearchPopup.OpenCmd.Execute(new object());
                args.Handled = true;
            }));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var datagrid = (DataGrid)sender;
            ((UserListViewModel)DataContext).SelectedItems = datagrid.SelectedItems;
        }
    }
}
