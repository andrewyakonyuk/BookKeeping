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

namespace BookKeeping.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        //private bool _shutdown;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = !_shutdown && ((MainWindowViewModel)DataContext).QuitConfirmationEnabled;
            //if (_shutdown) return;

            //var mySettings = new MetroDialogSettings()
            //{
            //    AffirmativeButtonText = "Quit",
            //    NegativeButtonText = "Cancel",
            //    AnimateShow = true,
            //    AnimateHide = false
            //};

            //var result = await this.ShowMessageAsync("Quit application?",
            //    "Sure you want to quit application?",
            //    MessageDialogStyle.AffirmativeAndNegative, mySettings);

            //_shutdown = result.Result == MessageDialogResult.Affirmative;
            
            //if (_shutdown)
            //    Application.Current.Shutdown();
        }
    }
}
