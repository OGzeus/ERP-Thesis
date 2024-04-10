using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using Erp.Model.Thesis.VacationPlanning;

namespace Erp.Converters
{
    public class CustomRowStyleConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value == 1)
                return "LightGreen";
            else
                return "White";
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
