using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MahApps.Metro.Converters;

namespace BookKeeping.UI.Converters
{
    public class InverseBooleanConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (!(bool)value);
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
