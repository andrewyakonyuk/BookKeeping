using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using ICommand = System.Windows.Input.ICommand;
using BookKeeping.UI.ViewModels;
using BookKeeping.UI;

namespace BookKeeping.App.ViewModels
{
    public class MainWindowViewModel : MainWindowViewModelBase
    {

        public MainWindowViewModel()
        {
            this.DisplayName = BookKeeping.App.Properties.Resources.Application_Name;

            Workspaces.Add(new WorkspaceViewModel
            {
                DisplayName = "Empty workspace"
            });

            OpenProductList = new DelegateCommand(_ =>
            {
                var viewModel = CreateOrExistWorkspace<ProductListViewModel>();
                SetActiveWorkspace(viewModel);
            });

            CollectionViewSource.GetDefaultView(Workspaces).MoveCurrentToFirst();
            Exit = ApplicationCommands.Close;
        }

        public ICommand AddProduct { get; set; }

        public ICommand OpenProductList { get; set; }

        public ICommand CloseTabItem { get; set; }

        public ICommand Exit { get; set; }

    }
}