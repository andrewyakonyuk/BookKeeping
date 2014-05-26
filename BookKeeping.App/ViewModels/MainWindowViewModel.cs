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
        private bool _quitConfirmationEnabled;

        public MainWindowViewModel()
        {
            this.DisplayName = BookKeeping.App.Properties.Resources.Application_Name;
            this.QuitConfirmationEnabled = true;

            Workspaces.Add(new WorkspaceViewModel
            {
                DisplayName = "Empty workspace"
            });

            CollectionViewSource.GetDefaultView(Workspaces).MoveCurrentToFirst();
            Exit = ApplicationCommands.Close;

            SaveCmd = new DelegateCommand(_ =>
            {
                if (CurrentWorkspace is ProductListViewModel)
                {
                    ((ProductListViewModel)CurrentWorkspace).SaveCmd.Execute(_);
                }
            },
            _ =>
            {
                if (CurrentWorkspace is ProductListViewModel)
                {
                    return ((ProductListViewModel)CurrentWorkspace).SaveCmd.CanExecute(_);
                }
                return false;
            });

            MainMenu.Clear();
            BuildMainMenu();
        }

        public ICommand CloseTabItem { get; set; }

        public ICommand Exit { get; set; }

        public ICommand ListOfProductsCmd { get; private set; }

        public ICommand ListOfCustomersCmd { get; private set; }

        public ICommand VendorListCmd { get; private set; }

        public ICommand SaleOfGoodsCmd { get; private set; }

        public ICommand PurchaseOfGoodsCmd { get; private set; }

        public ICommand SettingsCmd { get; private set; }

        public ICommand RemainsOfGoodsReportCmd { get; private set; }

        public ICommand HistoryOfGoodsReportCmd { get; private set; }

        public bool QuitConfirmationEnabled
        {
            get { return _quitConfirmationEnabled; }
            set
            {
                if (value.Equals(_quitConfirmationEnabled)) return;
                _quitConfirmationEnabled = value;
                OnPropertyChanged(() => QuitConfirmationEnabled);
            }
        }

        public ICommand SaveCmd { get; private set; }

        protected override void BuildMainMenu()
        {
            base.BuildMainMenu();

            ListOfProductsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<ProductListViewModel>()));
            SaleOfGoodsCmd = new DelegateCommand(_ => SetActiveWorkspace(CreateOrRetrieveWorkspace<OrderViewModel>()));

            //var fileNode = new MenuTreeNode(T("File"), null);
            //fileNode.AddChild(new MenuTreeNode(T("Product_List"), new DelegateCommand(_ =>
            //{
            //    var viewModel = CreateOrExistWorkspace<ProductListViewModel>();
            //    SetActiveWorkspace(viewModel);
            //})));
            //fileNode.AddChild(new MenuTreeNode(T("Basket"), new DelegateCommand(_ =>
            //{
            //    var viewModel = CreateOrExistWorkspace<OrderViewModel>();
            //    SetActiveWorkspace(viewModel);
            //})));

            //fileNode.AddChild(new MenuTreeNode(T("Save"), SaveCmd));

            //var editNode = new MenuTreeNode(T("Edit"), null);
            //editNode.AddChild(new MenuTreeNode(T("Undo"), ApplicationCommands.Undo));
            //editNode.AddChild(new MenuTreeNode(T("Redo"), ApplicationCommands.Redo));
            //editNode.AddChild(new MenuTreeNode(T("Cut"), ApplicationCommands.Cut));
            //editNode.AddChild(new MenuTreeNode(T("Copy"), ApplicationCommands.Copy));
            //editNode.AddChild(new MenuTreeNode(T("Paste"), ApplicationCommands.Paste));
            //editNode.AddChild(new MenuTreeNode(T("Find"), ApplicationCommands.Find));

            //var serviceNode = new MenuTreeNode(T("Service"), null);

            //var reportsNode = new MenuTreeNode(T("Reports"), null);
            //reportsNode.AddChild(new MenuTreeNode(T("RemainsOfGoods"), RemainsOfGoodsReportCmd));
            //reportsNode.AddChild(new MenuTreeNode(T("HistoryOfGoods")))

            //MainMenu.Add(fileNode);
            //MainMenu.Add(editNode);
        }

    }
}