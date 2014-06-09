﻿using BookKeeping.App.ViewModels;
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
    /// Interaction logic for CustomerListView.xaml
    /// </summary>
    public partial class CustomerListView : UserControl
    {
        public CustomerListView()
        {
            InitializeComponent();
            this.DataContextChanged += List_DataContextChanged;
        }

        public void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as CustomerListViewModel;
            if (viewModel.EditItemCmd.CanExecute(null))
                viewModel.EditItemCmd.Execute(((Control)sender).DataContext);
        }

        void List_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (CustomerListViewModel)DataContext;

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
            ((CustomerListViewModel)DataContext).SelectedItems = datagrid.SelectedItems;
        }
    }
}
