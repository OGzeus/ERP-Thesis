using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.Clusters;
using Erp.Model.SupplyChain.TSP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain
{
    public class TSP_OutputData:RecordBaseModel
    {

        private AntColony_TSP_OutputData _AntColony_outputdata;
        public AntColony_TSP_OutputData AntColony_Outputdata
        {
            get { return _AntColony_outputdata; }
            set
            {
                _AntColony_outputdata = value;
                INotifyPropertyChanged(nameof(AntColony_Outputdata));


            }
        }


        #region Diagrams

        private DiagramData _TSP_DiagramData;
        public DiagramData TSP_DiagramData
        {
            get { return _TSP_DiagramData; }
            set
            {
                _TSP_DiagramData = value;
                OnPropertyChanged(nameof(TSP_DiagramData));
            }
        }

        private ClusterDatapoint _SelectedCluster { get; set; }
        public ClusterDatapoint SelectedCluster
        {
            get { return _SelectedCluster; }
            set { _SelectedCluster = value; OnPropertyChanged("SelectedCluster"); }
        }

        private ObservableCollection<City_Tsp_OutputData> _CityTSPResults { get; set; }

        public ObservableCollection<City_Tsp_OutputData> CityTSPResults
        {
            get { return _CityTSPResults; }
            set { _CityTSPResults = value; OnPropertyChanged("CityTSPResults"); }
        }


        #endregion
    }
}
