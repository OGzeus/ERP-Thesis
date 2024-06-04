using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Erp.Model.SupplyChain
{
    public class ActivePanelToVisibilityConverter_VRP : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activePanel = (VRP_InputData.VRP_ActivePanel)value;
            var targetPanel = (VRP_InputData.VRP_ActivePanel)Enum.Parse(typeof(VRP_InputData.VRP_ActivePanel), (string)parameter);

            return activePanel == targetPanel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
