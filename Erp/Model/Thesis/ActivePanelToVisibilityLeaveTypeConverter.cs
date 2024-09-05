using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Erp.Model.Thesis;

namespace Erp.ViewModel.Thesis
{
    public class ActivePanelToVisibilityLeaveTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activePanel = (LeaveBidsData.ActivePanel)value;
            var targetPanel = (LeaveBidsData.ActivePanel)Enum.Parse(typeof(LeaveBidsData.ActivePanel), (string)parameter);

            return activePanel == targetPanel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
