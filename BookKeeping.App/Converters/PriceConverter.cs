using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookKeeping.UI.Converters;
using BookKeeping.Domain;

namespace BookKeeping.App.Converters
{
   public class PriceConverter : MarkupConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pieces = value.ToString().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string amount = string.Empty;
            string currency = string.Empty;

            if (pieces.Length > 2)
                throw new ArgumentException(T("InvalidPriceFormat"));

            if (pieces.Length > 0)
                amount = pieces[0];
            if (pieces.Length > 1)
                currency = pieces[1];

            Currency tempCurrency;
            decimal tempDecimal;
            if (decimal.TryParse(amount, System.Globalization.NumberStyles.Any, culture, out tempDecimal))
            {
                if (Enum.TryParse(currency, true, out tempCurrency))
                {
                    return new CurrencyAmount(tempDecimal, tempCurrency);
                }
                else return new CurrencyAmount(tempDecimal, Currency.Undefined);
            }
            throw new ArgumentException(T("InvalidPriceFormat"));
        }
    }
}
