using Erp.Model.Inventory;
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
    public class ActivePanelToVisibilityConverter_CL : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activePanel = (CL_InputData.CL_ActivePanel)value;
            var targetPanel = (CL_InputData.CL_ActivePanel)Enum.Parse(typeof(CL_InputData.CL_ActivePanel), (string)parameter);

            return activePanel == targetPanel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
