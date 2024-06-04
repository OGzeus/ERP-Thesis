using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.Clusters
{
    public class MainDatapoint : RecordBaseModel
    {
        private string _ClusterCode;
        private string code;
        private double longitude;
        private double latitude;
        private double _Demand;

        public string ClusterCode
        {
            get { return _ClusterCode; }
            set { _ClusterCode = value; OnPropertyChanged("ClusterCode"); }
        }
        public string Code
        {
            get { return code; }
            set { code = value; OnPropertyChanged("Code"); }
        }
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; OnPropertyChanged("Longitude"); }
        }
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; OnPropertyChanged("Latitude"); }
        }

        public double Demand
        {
            get { return _Demand; }
            set { _Demand = value; OnPropertyChanged("Demand"); }
        }
    }
}
