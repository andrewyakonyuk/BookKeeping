using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MahApps.Metro.Converters;
using System.Globalization;
using BookKeeping.App.Domain;
using System.Windows.Data;

namespace BookKeeping.App.Converters.Domain
{
    public class BarcodeConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var barcode = (Barcode)value;
            return barcode.ToString();
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
