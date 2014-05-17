using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MahApps.Metro.Converters;
using System.Globalization;

namespace BookKeeping.App.Converters
{
    public class SecurityLevelToVisibilityConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
