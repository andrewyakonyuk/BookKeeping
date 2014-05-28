using BookKeeping.Domain.Contracts;
using BookKeeping.UI.Converters;
using System;

namespace BookKeeping.App.Converters
{
  public  class BarcodeConverter : MarkupConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Barcode)
            {
                return ((Barcode)value).ToString();
            }
            return null;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var barcode = value.ToString();
            var pieces = barcode.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string code = string.Empty;
            string type = string.Empty;

            if (pieces.Length > 2)
                throw new ArgumentException(T("InvalidBarcodeFormat"));

            if (pieces.Length > 0)
                code = pieces[0];
            if (pieces.Length > 1)
                type = pieces[1];

            BarcodeType tempBarcodeType;
            if (Enum.TryParse<BarcodeType>(type, true, out tempBarcodeType))
            {
                return new Barcode(code, tempBarcodeType);
            }
            else return new Barcode(code, BarcodeType.Undefined);
        }
    }
}
