using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Erp.Converters
{
    public class RowStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush(Colors.Beige);

            var IsDeleted = (bool)value;
            if (IsDeleted == false)
                return new SolidColorBrush(Colors.Beige);
            else if (IsDeleted == true)
                return new SolidColorBrush(Colors.OrangeRed);
            else
                return new SolidColorBrush(Colors.LightGreen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
