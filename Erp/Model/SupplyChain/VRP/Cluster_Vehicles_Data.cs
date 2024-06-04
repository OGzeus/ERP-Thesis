using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.Clusters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.VRP
{
    public class Cluster_Vehicles_Data : RecordBaseModel
    {

        private ClusterDatapoint cluster;
        public ClusterDatapoint Cluster
        {
            get { return cluster; }
            set { cluster = value; OnPropertyChanged("Cluster"); }
        }

        private VehicleData vehicle;
        public VehicleData Vehicle
        {
            get { return vehicle; }
            set { vehicle = value; OnPropertyChanged("Vehicle"); }
        }
    }
}
