using BookKeeping.UI.ViewModels;
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
    /// Interaction logic for VendorDetailView.xaml
    /// </summary>
    public partial class VendorDetailView : UserControl
    {
       int _countOfErrors = 0;

       public VendorDetailView()
        {
            InitializeComponent();
        }

        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _countOfErrors++;
            else if (e.Action == ValidationErrorEventAction.Removed)
                _countOfErrors--;

            ((ViewModelBase)DataContext).IsValid = _countOfErrors <= 0;
        }
    }
}
