using Erp.Model.Data_Analytics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Community.CsharpSqlite.Sqlite3;

namespace Erp.ViewModel
{
    public class UpdateForecast : ViewModelBase
    {
        public UpdateForecast()
        {
            var Forecast = CommonFunctions.GetForecastInfoChooserData(5,null);
            var DemandForecast = new ObservableCollection<DemandForecastData>();

            // Calculate the number of days between DateFrom and DateTo
            var numberOfDays = (Forecast.DateTo - Forecast.DateFrom).Days;

            var Items = CommonFunctions.GetItemData(false);
            Items.Where(item => item.OutputOrderFlag == true);
            Items.RemoveAt(Items.Count - 1); // auto prepei na figei

            foreach (var item in Items)
            {
                if (Forecast.TimeBucket == Model.Enums.BasicEnums.Timebucket.Daily)
                {
                    for (int i = 0; i <= numberOfDays; i++)
                    {
                        var DForecastLine = new DemandForecastData();
                        DForecastLine.Demand = 50;
                        DForecastLine.Date = Forecast.DateFrom.AddDays(i);
                        DForecastLine.Item = item;

                        DemandForecast.Add(DForecastLine);
                    }
                }
            }

        }
    }

}
