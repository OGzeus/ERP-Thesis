using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Erp.Converters
{
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as string;

            var IsDeleted = (bool)value;
            //custom condition is checked based on data.

            if (IsDeleted == false)
                return new SolidColorBrush(Colors.Beige);
            else if (IsDeleted == true)
                return new SolidColorBrush(Colors.OrangeRed);
            else
                return new SolidColorBrush(Colors.LightGreen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}