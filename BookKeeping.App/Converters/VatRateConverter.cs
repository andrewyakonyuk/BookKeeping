using BookKeeping.Domain;
using BookKeeping.UI.Converters;
using MahApps.Metro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.Converters
{
    public class VatRateConverter : MarkupConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VatRate)
            {
                return ((VatRate)value).ToString(culture);
            }
            return null;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var percentSymbol = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol;
            var vatRate = value.ToString().Trim();

            if (vatRate.EndsWith(percentSymbol, true, culture))
            {
                vatRate = vatRate.ToString().Replace(percentSymbol, string.Empty);
                decimal tempDecimal;
                if (decimal.TryParse(vatRate, System.Globalization.NumberStyles.Float, culture, out tempDecimal))
                {
                    return new VatRate(tempDecimal / 100M);
                }
            }
            throw new ArgumentException(T("CannotParseAsDecimal"), "value");
        }
    }
}
