using MahApps.Metro.Converters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookKeeping.App.XamlExtensions
{
    public class ToLowerConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return val != null ? val.ToLower() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class ToUpperConverter : MarkupConverter
    {
        public string Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value != null ? Value.ToUpper() : string.Empty;
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return val != null ? val.ToUpper() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}