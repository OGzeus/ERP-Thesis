using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Erp.Model.Data_Analytics.Forecast;

namespace Erp.Converters
{
    internal class CustomRowStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var row = (item as DataRowBase).RowData;
            var data = row as ForecastInfoData;

            if (data.IsDeleted ==false)
                return App.Current.Resources["rowStyle1"] as Style;
            return App.Current.Resources["rowStyle2"] as Style;
        }
    }
}
