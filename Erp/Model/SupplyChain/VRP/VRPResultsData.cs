using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.VRP
{
    public class VRPResultsData : RecordBaseModel
    {
        private ObservableCollection<RouteVRPData> _Routes;
        public ObservableCollection<RouteVRPData> Routes
        {
            get { return _Routes; }
            set { _Routes = value; OnPropertyChanged("Routes"); }
        }
        private double _TotalCost { get; set; }
        private int _Iterations { get; set; }
        public double TotalCost
        {
            get { return _TotalCost; }
            set { _TotalCost = value; OnPropertyChanged("TotalCost"); }
        }
        public int Iterations
        {
            get { return _Iterations; }
            set { _Iterations = value; OnPropertyChanged("Iterations"); }
        }
    }
}
