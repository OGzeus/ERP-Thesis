using Erp.Model.Data_Analytics;
using Erp.Model.Inventory;
using Erp.Model.Manufacture;
using Erp.Repositories;
using Erp.View.Manufacture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.ViewModel.Data_Analytics
{

    public class MRPVisualisationViewModel :ViewModelBase
    {

        private ObservableCollection<HeatMapData> heatmapdata { get; set; }
        public ObservableCollection<HeatMapData> HeatmapData
        {
            get { return heatmapdata; }
            set
            {
                heatmapdata = value;
                INotifyPropertyChanged(nameof(HeatmapData));
            }
        }
        private ObservableCollection<PlannedOrdersData> plannedordersdata {get; set; }
        public ObservableCollection<PlannedOrdersData> PlannedOrdersData
        {
            get { return plannedordersdata; }
            set
            {
                plannedordersdata = value;
                INotifyPropertyChanged(nameof(PlannedOrdersData));
            }
        }

        public MRPVisualisationViewModel()
        {
            HeatmapData = new ObservableCollection<HeatMapData>();

            PlannedOrdersData = new ObservableCollection<PlannedOrdersData>();
            HeatmapData = CommonFunctions.GetPlannedOrdersData();

        }
    }

 }