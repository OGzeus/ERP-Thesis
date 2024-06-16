using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.Clusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.TSP
{
    public class City_Tsp_OutputData : RecordBaseModel
    {
        private CityData _City;
        public CityData City
        {
            get { return _City; }
            set { _City = value; OnPropertyChanged("City"); }
        }

        private ClusterDatapoint _Cluster;
        public ClusterDatapoint Cluster
        {
            get { return _Cluster; }
            set { _Cluster = value; OnPropertyChanged("Cluster"); }
        }

        private int _Number_Visited;
        public int Number_Visited
        {   
            get { return _Number_Visited; }
            set { _Number_Visited = value; OnPropertyChanged("Number_Visited"); }
        }
        private double _BestTourLength;
        public double BestTourLength
        {
            get { return _BestTourLength; }
            set
            {
                _BestTourLength = value;
                INotifyPropertyChanged(nameof(BestTourLength));


            }
        }

    }
}
