using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Charts;
using System.Threading.Tasks;
using static Erp.Model.Data_Analytics.SalesDashBoardData;

namespace Erp.ViewModel.Data_Analytics
{
    public class SalesDashBoardViewModel : ViewModelBase
    {
        public ObservableCollection<PieChartData> PieChartData { get; set; }
        public ObservableCollection<BarChartData> BarChartData { get; set; }

        public SalesDashBoardViewModel()
        {
            PieChartData = new ObservableCollection<PieChartData>();
            BarChartData = new ObservableCollection<BarChartData>();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Load Pie Chart Data
            PieChartData.Add(new PieChartData { Item = "Item A", Profit = 10000 });
            PieChartData.Add(new PieChartData { Item = "Item B", Profit = 20000 });
            PieChartData.Add(new PieChartData { Item = "Item C", Profit = 15000 });

            // Load Bar Chart Data
            BarChartData.Add(new BarChartData { City = "City A", Orders = 50, Profit = 10000 });
            BarChartData.Add(new BarChartData { City = "City B", Orders = 70, Profit = 20000 });
            BarChartData.Add(new BarChartData { City = "City C", Orders = 30, Profit = 15000 });
        }
    }
}

