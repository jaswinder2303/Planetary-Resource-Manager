using System;
using System.Globalization;
using System.Windows.Data;

namespace PlanetaryResourceManager.Converters
{
    public class NumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var money = double.Parse(value.ToString());
            return money.ToString("N");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
